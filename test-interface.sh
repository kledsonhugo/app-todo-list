#!/bin/bash

# Interface Tests for Todo List Application
# This script tests all interface interactions with backend endpoints

echo "🧪 Todo List Interface Tests"
echo "===================================="

# Function to check if app is running
check_app() {
    curl -s -o /dev/null -w "%{http_code}" http://localhost:5146 2>/dev/null
}

# Function to wait for app to start
wait_for_app() {
    echo "⏳ Waiting for application to start..."
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if [ "$(check_app)" = "200" ]; then
            echo "✅ Application is running on http://localhost:5146"
            return 0
        fi
        echo "   Attempt $attempt/$max_attempts - waiting..."
        sleep 1
        ((attempt++))
    done
    
    echo "❌ Application failed to start within 30 seconds"
    return 1
}

# Function to run a test
run_test() {
    local test_name="$1"
    local test_command="$2"
    local expected_code="$3"
    
    echo ""
    echo "🔍 Test: $test_name"
    
    result=$(eval "$test_command")
    exit_code=$?
    
    if [ $exit_code -eq 0 ] && [[ $result =~ $expected_code ]]; then
        echo "✅ PASS"
        return 0
    else
        echo "❌ FAIL"
        echo "   Expected: $expected_code"
        echo "   Result: $result"
        return 1
    fi
}

# Function to test JSON response
test_json() {
    local test_name="$1"
    local url="$2"
    local method="${3:-GET}"
    local data="${4:-}"
    local expected_pattern="$5"
    
    echo ""
    echo "🔍 Test: $test_name"
    
    if [ "$method" = "GET" ]; then
        response=$(curl -s -w "\n%{http_code}" "$url")
    elif [ "$method" = "POST" ]; then
        response=$(curl -s -w "\n%{http_code}" -X POST -H "Content-Type: application/json" -d "$data" "$url")
    elif [ "$method" = "PUT" ]; then
        response=$(curl -s -w "\n%{http_code}" -X PUT -H "Content-Type: application/json" -d "$data" "$url")
    elif [ "$method" = "PATCH" ]; then
        response=$(curl -s -w "\n%{http_code}" -X PATCH "$url")
    elif [ "$method" = "DELETE" ]; then
        response=$(curl -s -w "\n%{http_code}" -X DELETE "$url")
    fi
    
    # Split response body and status code
    body=$(echo "$response" | head -n -1)
    status=$(echo "$response" | tail -n 1)
    
    if [[ $body =~ $expected_pattern ]] && [ "$status" = "200" -o "$status" = "201" -o "$status" = "204" ]; then
        echo "✅ PASS (HTTP $status)"
        if [ "$method" = "POST" ] && [ -n "$body" ]; then
            # Extract ID from created todo for later use
            created_id=$(echo "$body" | sed -n 's/.*"id":\([0-9]*\).*/\1/p')
            echo "   Created ID: $created_id"
            echo "$created_id" > /tmp/last_created_id
        fi
        return 0
    else
        echo "❌ FAIL (HTTP $status)"
        echo "   Body: $body"
        return 1
    fi
}

# Start the application in background
echo "🚀 Starting Todo List Application..."
cd /home/runner/work/app-todo-list/app-todo-list
dotnet run --project TodoListApp.csproj > /tmp/app.log 2>&1 &
APP_PID=$!

# Store PID for cleanup
echo $APP_PID > /tmp/app.pid

# Wait for app to start
if ! wait_for_app; then
    kill $APP_PID 2>/dev/null
    exit 1
fi

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0

echo ""
echo "📋 Running Interface Tests..."
echo "===================================="

# Test 1: Web page loads successfully
((TOTAL_TESTS++))
if run_test "Web page loads with correct title" \
    "curl -s -w '%{http_code}' http://localhost:5146 | grep -q 'Todo List - Gerenciador de Tarefas' && echo '200'" \
    "200"; then
    ((PASSED_TESTS++))
fi

# Test 2: CSS file loads
((TOTAL_TESTS++))
if run_test "CSS file loads correctly" \
    "curl -s -w '%{http_code}' -o /dev/null http://localhost:5146/styles.css" \
    "200"; then
    ((PASSED_TESTS++))
fi

# Test 3: JavaScript file loads
((TOTAL_TESTS++))
if run_test "JavaScript file loads correctly" \
    "curl -s -w '%{http_code}' -o /dev/null http://localhost:5146/script.js" \
    "200"; then
    ((PASSED_TESTS++))
fi

# Test 4: JavaScript contains interface functions
((TOTAL_TESTS++))
js_functions=$(curl -s http://localhost:5146/script.js | grep -o 'loadTodos\|createTodo\|updateTodo\|deleteTodo' | wc -l)
echo ""
echo "🔍 Test: JavaScript contains interface functions"
if [ "$js_functions" -ge 4 ]; then
    echo "✅ PASS (Found $js_functions interface functions)"
    ((PASSED_TESTS++))
else
    echo "❌ FAIL (Found only $js_functions interface functions)"
fi

# Test 5: API endpoint - Get initial todos
((TOTAL_TESTS++))
if test_json "Get initial todos (interface loads these)" \
    "http://localhost:5146/api/todos" \
    "GET" \
    "" \
    '"title".*"Estudar .NET"'; then
    ((PASSED_TESTS++))
fi

# Test 6: API endpoint - Create new todo (interface form submission)
((TOTAL_TESTS++))
if test_json "Create new todo via interface API" \
    "http://localhost:5146/api/todos" \
    "POST" \
    '{"title": "Interface Test Todo", "description": "Created by interface test"}' \
    '"title".*"Interface Test Todo"'; then
    ((PASSED_TESTS++))
fi

# Test 7: API endpoint - Update todo (interface edit form)
if [ -f /tmp/last_created_id ]; then
    created_id=$(cat /tmp/last_created_id)
    ((TOTAL_TESTS++))
    if test_json "Update todo via interface API" \
        "http://localhost:5146/api/todos/$created_id" \
        "PUT" \
        '{"title": "Updated Interface Test", "description": "Updated via interface", "isCompleted": false}' \
        '"title".*"Updated Interface Test"'; then
        ((PASSED_TESTS++))
    fi
    
    # Test 8: API endpoint - Toggle completion (interface toggle button)
    ((TOTAL_TESTS++))
    if test_json "Toggle todo completion via interface" \
        "http://localhost:5146/api/todos/$created_id/toggle" \
        "PATCH" \
        "" \
        '"isCompleted".*true'; then
        ((PASSED_TESTS++))
    fi
    
    # Test 9: API endpoint - Delete todo (interface delete button)
    ((TOTAL_TESTS++))
    echo ""
    echo "🔍 Test: Delete todo via interface API"
    delete_response=$(curl -s -w "\n%{http_code}" -X DELETE "http://localhost:5146/api/todos/$created_id")
    delete_status=$(echo "$delete_response" | tail -n 1)
    
    if [ "$delete_status" = "204" ]; then
        echo "✅ PASS (HTTP $delete_status)"
        ((PASSED_TESTS++))
    else
        echo "❌ FAIL (HTTP $delete_status)"
    fi
fi

# Test 10: Error handling - 404 for non-existent todo
((TOTAL_TESTS++))
echo ""
echo "🔍 Test: Error handling for non-existent todo"
not_found_response=$(curl -s -w "%{http_code}" -o /dev/null "http://localhost:5146/api/todos/999999")

if [ "$not_found_response" = "404" ]; then
    echo "✅ PASS (HTTP 404)"
    ((PASSED_TESTS++))
else
    echo "❌ FAIL (HTTP $not_found_response)"
fi

# Test 11: Validation error - empty title
((TOTAL_TESTS++))
echo ""
echo "🔍 Test: Validation error for empty title"
validation_response=$(curl -s -w "\n%{http_code}" -X POST -H "Content-Type: application/json" -d '{"description": "No title"}' "http://localhost:5146/api/todos")
validation_status=$(echo "$validation_response" | tail -n 1)

if [ "$validation_status" = "400" ]; then
    echo "✅ PASS (HTTP 400)"
    ((PASSED_TESTS++))
else
    echo "❌ FAIL (HTTP $validation_status)"
fi

# Test 12: End-to-end workflow simulation
((TOTAL_TESTS++))
echo ""
echo "🔍 Test: End-to-end interface workflow"

# Simulate complete user workflow
workflow_success=true
workflow_errors=""

# 1. Get initial count
initial_response=$(curl -s "http://localhost:5146/api/todos")
initial_count=$(echo "$initial_response" | grep -o '"id"' | wc -l)
echo "   Initial todo count: $initial_count"

# 2. Create todo
create_response=$(curl -s -w "\n%{http_code}" -X POST -H "Content-Type: application/json" -d '{"title": "E2E Test", "description": "End-to-end test"}' "http://localhost:5146/api/todos")
create_status=$(echo "$create_response" | tail -n 1)
create_body=$(echo "$create_response" | head -n -1)

if [ "$create_status" != "201" ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Create failed (HTTP $create_status)."
else
    echo "   ✓ Todo created successfully"
fi

e2e_id=$(echo "$create_body" | sed -n 's/.*"id":\([0-9]*\).*/\1/p')
echo "   Created todo ID: $e2e_id"

# 3. Verify count increased
updated_response=$(curl -s "http://localhost:5146/api/todos")
updated_count=$(echo "$updated_response" | grep -o '"id"' | wc -l)
echo "   Updated todo count: $updated_count"

if [ $updated_count -ne $((initial_count + 1)) ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Count not increased correctly ($updated_count vs expected $((initial_count + 1)))."
else
    echo "   ✓ Todo count increased correctly"
fi

# 4. Update todo
update_response=$(curl -s -w "%{http_code}" -o /dev/null -X PUT -H "Content-Type: application/json" -d '{"title": "E2E Updated", "description": "Updated", "isCompleted": false}' "http://localhost:5146/api/todos/$e2e_id")
if [ "$update_response" != "200" ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Update failed (HTTP $update_response)."
else
    echo "   ✓ Todo updated successfully"
fi

# 5. Toggle completion
toggle_response=$(curl -s -w "%{http_code}" -o /dev/null -X PATCH "http://localhost:5146/api/todos/$e2e_id/toggle")
if [ "$toggle_response" != "200" ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Toggle failed (HTTP $toggle_response)."
else
    echo "   ✓ Todo completion toggled successfully"
fi

# 6. Delete todo
delete_response=$(curl -s -w "%{http_code}" -o /dev/null -X DELETE "http://localhost:5146/api/todos/$e2e_id")
if [ "$delete_response" != "204" ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Delete failed (HTTP $delete_response)."
else
    echo "   ✓ Todo deleted successfully"
fi

# 7. Verify count returned to initial
final_response=$(curl -s "http://localhost:5146/api/todos")
final_count=$(echo "$final_response" | grep -o '"id"' | wc -l)
echo "   Final todo count: $final_count"

if [ $final_count -ne $initial_count ]; then
    workflow_success=false
    workflow_errors="$workflow_errors Final count incorrect ($final_count vs expected $initial_count)."
else
    echo "   ✓ Todo count returned to initial value"
fi

if $workflow_success; then
    echo "✅ PASS"
    ((PASSED_TESTS++))
else
    echo "❌ FAIL"
    echo "   Errors: $workflow_errors"
fi

# Cleanup
echo ""
echo "🧹 Cleaning up..."
kill $APP_PID 2>/dev/null
rm -f /tmp/app.pid /tmp/last_created_id

# Results
echo ""
echo "📊 Test Results"
echo "===================================="
echo "Total Tests: $TOTAL_TESTS"
echo "Passed: $PASSED_TESTS"
echo "Failed: $((TOTAL_TESTS - PASSED_TESTS))"

if [ $PASSED_TESTS -eq $TOTAL_TESTS ]; then
    echo ""
    echo "🎉 All interface tests passed!"
    echo "✅ Web interface successfully integrates with backend APIs"
    echo "✅ All CRUD operations work through interface endpoints"
    echo "✅ Error handling works correctly"
    echo "✅ End-to-end workflow validation successful"
    exit 0
else
    echo ""
    echo "❌ Some tests failed. Check the output above for details."
    exit 1
fi