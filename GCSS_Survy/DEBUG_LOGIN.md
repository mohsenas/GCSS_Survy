# Debugging Login Endpoint Issue

## Problem
The login endpoint at `api/GCSS_Survy/SecureAuth/v1/login` is hanging - requests are sent but no response is received.

## Changes Made
1. ✅ Enabled Swagger UI for endpoint discovery
2. ✅ Lowered logging level to Information for better debugging
3. ✅ Fixed middleware order (exception handling early in pipeline)
4. ✅ Added diagnostic logging to SecureAuthController

## Debugging Steps

### 1. Verify Application is Running
- Check console output for startup messages
- Verify the application is listening on `http://localhost:5000` or `https://localhost:5001`
- Look for any error messages during startup

### 2. Check Swagger UI
Navigate to: `http://localhost:5000/swagger` (or `https://localhost:5001/swagger`)

Look for:
- `POST /api/GCSS_Survy/SecureAuth/v1/login` endpoint
- Required request body format
- Expected parameters (username, password, branchId, etc.)

### 3. Verify Database Connection
The connection string is:
```
Server=.;Database=GCSS_Survy;MultipleActiveResultSets=False;TrustServerCertificate=True;Trusted_Connection=True;Pooling=true;Max Pool Size=100;Connection Timeout=30;
```

**Check:**
- SQL Server is running
- Database `GCSS_Survy` exists
- Windows Authentication is working
- Connection timeout is not being exceeded

### 4. Test with Postman

**Endpoint URL:**
```
POST http://localhost:5000/api/GCSS_Survy/SecureAuth/v1/login
```

**Headers:**
```
Content-Type: application/json
```

**Expected Request Body (check Swagger for exact format):**
```json
{
  "username": "admin",
  "password": "your-password",
  "branchId": 1
}
```

**Note:** The seeded user has:
- Username: `admin`
- Password: Hashed (check DataSeeder.cs for the hash)
- DefaultBranchID: 1

### 5. Check Console Logs
With Information-level logging enabled, you should see:
- Request received logs
- Database query logs
- Any exceptions or errors
- Controller initialization messages

### 6. Common Issues to Check

#### Database Connection Timeout
- Verify SQL Server is accessible
- Check if database exists: `SELECT name FROM sys.databases WHERE name = 'GCSS_Survy'`
- Test connection manually

#### Rate Limiting
The endpoint has `[EnableRateLimiting("Authentication")]` which limits to 10 requests per minute.
- If you've made multiple failed attempts, wait 1 minute before retrying

#### Missing Request Body
- Ensure Content-Type header is set to `application/json`
- Verify the request body matches the expected format from Swagger

#### Async/Await Issues
- Check if any async operations are not completing
- Look for deadlocks in database queries

### 7. Test Database Connection
Run this SQL query to verify the user exists:
```sql
SELECT ID, UserName, DefaultBranchID, IsEnabled 
FROM Users 
WHERE UserName = 'admin'
```

### 8. Check for Exceptions
- Review console output for any exception stack traces
- Check if exception handling middleware is catching errors
- Look for database-related errors

## Next Steps
1. Run the application and check Swagger UI
2. Verify the exact request format from Swagger
3. Test with Postman using the correct format
4. Monitor console logs for any errors or hanging operations
5. Verify database connectivity

## If Still Hanging
1. Add breakpoints in the BaseAuthenticationController login method (if accessible)
2. Check if the request is reaching the controller (look for initialization log)
3. Verify all dependencies are properly injected
4. Check for circular dependencies or missing service registrations

