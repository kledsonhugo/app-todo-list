# Todo List Interface Tests

This document describes the interface tests implemented for the Todo List application to validate all interactions between the web interface and backend API endpoints.

## Overview

The interface tests validate that all web page interactions with the backend API work correctly. These tests simulate the exact HTTP requests that the JavaScript interface makes when users interact with the web application.

## Test Coverage

### 🌐 Web Interface Integration
- **Page Loading**: Validates that the main HTML page loads with correct title and elements
- **Static Resources**: Tests that CSS and JavaScript files are served properly
- **JavaScript Functions**: Verifies that the interface contains all required functions

### 📡 API Endpoint Integration
- **GET /api/todos**: Tests loading todos (used when page loads)
- **POST /api/todos**: Tests creating new todos (used when form is submitted)
- **PUT /api/todos/{id}**: Tests updating todos (used by edit modal)
- **PATCH /api/todos/{id}/toggle**: Tests toggling completion (used by toggle buttons)
- **DELETE /api/todos/{id}**: Tests deleting todos (used by delete buttons)

### 🔍 Error Handling
- **404 Errors**: Tests handling of non-existent todos
- **400 Validation**: Tests form validation (empty title)
- **Error Responses**: Validates proper HTTP status codes

### 🔄 End-to-End Workflow
- Complete user journey simulation
- Create → Read → Update → Toggle → Delete workflow
- Data consistency validation

## Running the Tests

### Prerequisites
- .NET 8.0 SDK
- curl command-line tool
- Bash shell

### Execution
```bash
# Make the test script executable
chmod +x test-interface.sh

# Run the interface tests
./test-interface.sh
```

### Test Process
1. **Application Startup**: Automatically starts the Todo List application
2. **Connection Wait**: Waits for application to be ready (up to 30 seconds)
3. **Test Execution**: Runs all 12 interface tests
4. **Cleanup**: Automatically stops the application
5. **Results**: Displays pass/fail results with detailed output

## Test Details

### Test 1: Web Page Loading
- **Purpose**: Validates the main HTML page loads correctly
- **Method**: GET request to `/`
- **Validation**: Checks for "Todo List - Gerenciador de Tarefas" title

### Test 2: CSS File Loading
- **Purpose**: Ensures stylesheet is served correctly
- **Method**: GET request to `/styles.css`
- **Validation**: HTTP 200 response

### Test 3: JavaScript File Loading
- **Purpose**: Ensures JavaScript is served correctly
- **Method**: GET request to `/script.js`
- **Validation**: HTTP 200 response

### Test 4: JavaScript Interface Functions
- **Purpose**: Validates JavaScript contains required interface functions
- **Method**: Content analysis of `/script.js`
- **Validation**: Checks for loadTodos, createTodo, updateTodo, deleteTodo functions

### Test 5: Initial Todos Loading
- **Purpose**: Tests the API endpoint used when page loads
- **Method**: GET request to `/api/todos`
- **Validation**: Returns initial todos including "Estudar .NET"

### Test 6: Todo Creation
- **Purpose**: Tests the API endpoint used by the add todo form
- **Method**: POST request to `/api/todos`
- **Data**: `{"title": "Interface Test Todo", "description": "Created by interface test"}`
- **Validation**: HTTP 201 with created todo data

### Test 7: Todo Update
- **Purpose**: Tests the API endpoint used by the edit modal
- **Method**: PUT request to `/api/todos/{id}`
- **Data**: Updated title, description, and completion status
- **Validation**: HTTP 200 with updated todo data

### Test 8: Completion Toggle
- **Purpose**: Tests the API endpoint used by toggle buttons
- **Method**: PATCH request to `/api/todos/{id}/toggle`
- **Validation**: HTTP 200 with toggled completion status

### Test 9: Todo Deletion
- **Purpose**: Tests the API endpoint used by delete buttons
- **Method**: DELETE request to `/api/todos/{id}`
- **Validation**: HTTP 204 (No Content)

### Test 10: 404 Error Handling
- **Purpose**: Tests error handling for non-existent todos
- **Method**: GET request to `/api/todos/999999`
- **Validation**: HTTP 404 response

### Test 11: Validation Error Handling
- **Purpose**: Tests form validation for required fields
- **Method**: POST request with empty title
- **Data**: `{"description": "No title"}`
- **Validation**: HTTP 400 (Bad Request)

### Test 12: End-to-End Workflow
- **Purpose**: Simulates complete user interaction workflow
- **Steps**:
  1. Get initial todo count
  2. Create new todo
  3. Verify count increased
  4. Update the todo
  5. Toggle completion
  6. Delete the todo
  7. Verify count returned to initial
- **Validation**: All operations succeed and data consistency is maintained

## Success Criteria

All tests must pass for the interface to be considered properly integrated:

✅ **12/12 Tests Passing**
- Web page loads correctly
- Static resources served properly
- JavaScript contains interface functions
- All CRUD operations work through API
- Error handling functions correctly
- End-to-end workflow completes successfully

## Benefits

These interface tests provide:

1. **Integration Validation**: Ensures web interface properly communicates with backend
2. **API Contract Testing**: Validates all API endpoints work as expected
3. **User Journey Verification**: Tests complete user workflows
4. **Error Handling**: Validates proper error responses
5. **Regression Prevention**: Catches breaking changes in interface integration
6. **Confidence**: Ensures the web interface works reliably

## Technical Implementation

The tests use:
- **Bash Scripting**: For test orchestration and execution
- **curl**: For HTTP request simulation
- **Background Processes**: For application lifecycle management
- **JSON Parsing**: For response validation
- **Status Code Validation**: For HTTP response verification

This approach provides comprehensive validation of the Todo List web interface's integration with its backend API endpoints, ensuring all user interactions work correctly.