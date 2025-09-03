# 🧪 Automated Interface Testing with MCP

## Overview

This testing framework implements comprehensive automated interface testing using MCP (Model Context Protocol) for the Todo List application. It acts as an integrated QA system to improve quality and accelerate the development cycle.

## Features

### 🎯 Test Coverage
- **Interface Load and Structure Tests** - Validates page loads and UI components
- **Add Todo Functionality** - Tests form validation and task creation
- **Filter Operations** - Validates filtering by All, Pending, and Completed
- **Task Actions** - Tests Complete, Edit, Delete operations
- **Refresh Functionality** - Validates data synchronization
- **Responsive Design** - Tests mobile and desktop layouts
- **Error Handling** - Validates graceful error handling
- **Performance Testing** - Measures load times and responsiveness
- **API Integration** - Tests frontend-backend communication

### 🛠 Technology Stack
- **NUnit** - Test framework
- **Playwright** - Browser automation
- **ASP.NET Core Testing** - Integration testing
- **GitHub Actions** - CI/CD automation

## Project Structure

```
tests/
├── TestProject.csproj           # Test project configuration
├── TodoListInterfaceTests.cs    # Main UI test suite
├── TodoListApiTests.cs          # API integration tests
├── TestHelpers.cs               # Test utilities and helpers
├── PlaywrightSetup.cs           # Browser automation setup
├── test.runsettings             # Test execution configuration
└── TestResults/                 # Test output directory
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Chromium browser (auto-installed by Playwright)

### Installation

1. **Install dependencies:**
   ```bash
   cd tests
   dotnet restore
   dotnet build
   ```

2. **Install Playwright browsers:**
   ```bash
   # From the project root
   ./run-tests.sh install
   ```

### Running Tests

#### Quick Test Execution
```bash
# Run all tests
./run-tests.sh

# Run only API tests
./run-tests.sh api

# Run only interface tests
./run-tests.sh interface

# Generate test report
./run-tests.sh report
```

#### Manual Test Execution
```bash
# Start the application
dotnet run

# In another terminal, run tests
cd tests
dotnet test
```

## Test Categories

### 🌐 Interface Tests (`TodoListInterfaceTests`)

#### Page Load Tests
- Validates page title and main heading
- Verifies all sections are present
- Checks initial sample tasks load

#### Form Validation Tests
- Tests required field validation
- Validates form submission
- Checks form clearing after submission

#### Filter Tests
- Tests "All" filter (default state)
- Validates "Pending" filter functionality
- Verifies "Completed" filter behavior

#### Task Action Tests
- Complete/Reopen task operations
- Edit task functionality with modal
- Delete task with confirmation

#### Responsive Design Tests
- Mobile viewport testing
- Button accessibility on touch devices

#### Performance Tests
- Page load time validation
- Interaction responsiveness testing

#### Error Handling Tests
- Network error simulation
- Graceful degradation testing

### 🔧 API Integration Tests (`TodoListApiTests`)

#### CRUD Operations
- GET all todos
- GET todo by ID
- POST new todo
- PUT update todo
- PATCH toggle completion
- DELETE todo

#### Validation Tests
- Required field validation
- Data type validation
- Content length limits

#### Response Tests
- Status code validation
- Response format verification
- Header validation

## Configuration

### Test Settings (`test.runsettings`)
- Maximum CPU count: 1 (sequential execution)
- Default timeout: 60 seconds
- Verbosity level: 2 (detailed)
- Code coverage collection enabled

### Browser Configuration
- **Headless mode**: Enabled for CI/CD
- **Viewport**: 1280x720 (desktop testing)
- **Locale**: pt-BR (Brazilian Portuguese)
- **Timezone**: America/Sao_Paulo

## CI/CD Integration

### GitHub Actions Workflow
The `.github/workflows/interface-testing.yml` workflow provides:

1. **Automated Testing**
   - Builds application and tests
   - Installs Playwright browsers
   - Runs comprehensive test suite

2. **Quality Assurance**
   - API integration testing
   - Interface functionality validation
   - Performance benchmarking

3. **Accessibility Testing**
   - pa11y accessibility validation
   - WCAG compliance checking

4. **Performance Testing**
   - Lighthouse performance audits
   - Budget threshold validation

## Test Data

### Sample Data
The application starts with 3 sample todos:
1. "Estudar .NET" (pending)
2. "Fazer exercícios" (completed)
3. "Ler documentação" (pending)

### Test Utilities
- `TestHelpers.GenerateTestTodoData()` - Creates unique test data
- `TestHelpers.Selectors` - Common CSS selectors
- `TestHelpers.Timeouts` - Standard wait times
- `TestHelpers.ExpectedData` - Expected test values

## Debugging

### Local Debugging
1. Set `Headless = false` in `PlaywrightSetup.cs`
2. Add breakpoints in test methods
3. Run tests in debug mode

### Test Output
- Console logs captured from browser
- Screenshots saved on test failures
- Detailed error messages and stack traces

### Common Issues

#### Browser Installation
```bash
# If browsers fail to install
./run-tests.sh install
```

#### Port Conflicts
```bash
# Check if port 5146 is available
netstat -tulpn | grep 5146
```

#### Test Timeouts
- Increase timeout values in `TestHelpers.Timeouts`
- Check application startup time
- Verify network connectivity

## Best Practices

### Test Writing
1. **Descriptive Names** - Use clear, descriptive test method names
2. **Arrange-Act-Assert** - Follow AAA pattern for test structure
3. **Independent Tests** - Each test should be self-contained
4. **Stable Selectors** - Use data attributes or stable CSS classes

### Performance
1. **Minimal DOM Queries** - Cache element references when possible
2. **Appropriate Waits** - Use explicit waits over implicit timeouts
3. **Resource Cleanup** - Ensure proper browser cleanup after tests

### Maintenance
1. **Regular Updates** - Keep test dependencies updated
2. **Test Review** - Regularly review and refactor tests
3. **Documentation** - Update documentation with new test scenarios

## Extending Tests

### Adding New Test Cases
1. Create new test methods in appropriate test classes
2. Use existing test helpers and utilities
3. Follow established naming conventions
4. Update documentation

### Custom Test Helpers
```csharp
public static class CustomTestHelpers
{
    public static async Task WaitForToastMessage(IPage page, string message)
    {
        await page.WaitForSelectorAsync($".toast:has-text('{message}')");
    }
}
```

### Integration with External Services
For testing external integrations:
1. Use test doubles/mocks
2. Create dedicated test environments
3. Implement proper test data isolation

## Reporting

### Test Results
- **Console Output** - Real-time test progress
- **TRX Files** - Detailed test results
- **Coverage Reports** - Code coverage analysis
- **HTML Reports** - Visual test summaries

### Metrics Tracked
- Test execution time
- Pass/fail rates
- Code coverage percentage
- Performance benchmarks

## Support

### Troubleshooting
1. Check application logs: `/tmp/server.log`
2. Review test output in `TestResults/`
3. Verify browser installation
4. Check port availability

### Contributing
1. Follow existing test patterns
2. Add appropriate test coverage
3. Update documentation
4. Run full test suite before committing

## Future Enhancements

### Planned Features
- Visual regression testing
- Cross-browser testing (Firefox, Safari)
- Mobile device emulation
- API load testing
- Security testing integration

### Integration Opportunities
- Docker container testing
- Database integration tests
- Email notification testing
- File upload/download testing

This comprehensive testing framework ensures the Todo List application maintains high quality through automated validation of all user-facing functionality and API integrations.