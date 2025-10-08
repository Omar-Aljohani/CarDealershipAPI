# CarDealershipAPI (.NET 9)

## How to run
1. Ensure .NET 9 SDK installed.
2. `dotnet restore`
3. `dotnet run`
4. Visit Swagger UI (in Development): `http://localhost:5102/swagger/index.html`
(port provided by Kestrel)

## Important Note
There is a postman collection `CarDealershipAPI.postman_collection.json` included which contains the valid path of calls to demonstrate the API, organized in steps and providing details of the request body and headers. Use it to test the API easily.
Additionally each step folder will contain in its description any additional requirment or changes that need to be done to the request that belongs to it. Like inserting the OTP value or adding a header. For the ease of testing I heavily recommend using this collection.
You can either download the desktop app and import the collection - recommanded - or use the web version of Postman and view the published verison throught this link https://documenter.getpostman.com/view/34833096/2sB3QKq9Gr.
Nevertheless, the endpoints and flows are documented below.

## Endpoints
### Authentication
- `POST /api/auth/register` - create user, will send OTP but it's not required to confirm it's a vaild user. You can login right away.
- `POST /api/auth/login` - provide credentials, receives OTP (simulate) then
call `POST /api/auth/validate-otp` with action `Login` and OTP to receive JWT
- `POST /api/auth/request-otp` - request OTP for given action (Register, Login,
PurchaseRequest, UpdateVehicle) You will need it for the respective action.
- `POST /api/auth/validate-otp` - validate OTP; returns `otpToken` used in
header `X-OTP-Token` for protected actions
### Vehicles
- `GET /api/vehicles` - browse available vehicles with filters
- `GET /api/vehicles/{id}` - vehicle details
- `POST /api/vehicles` - Admin create vehicle (JWT Admin role)
- `PUT /api/vehicles/{id}` - Admin update vehicle (JWT Admin role + OTP token in `X-OTP-Token`)
### Sales
- `POST /api/sales/request-purchase` - Customer purchase request (JWT Customer + OTP token header)
- `GET /api/sales/history` - view purchase history (Customer sees own history; Admin sees all)
- `POST /api/sales/process-sale` - Admin: finalize sale, this path require the sale ID to mark it as processed
- `POST /api/sales/add-sale` - Admin: add a sale directly for a customer, can be used for walk-in customers.
### Admin
- `GET /api/admin/customers` - Admin: list customers, with sales.

## OTP flow (detailed)
1. Client calls `POST /api/auth/request-otp` with `{ "email": "user@x.com",
"action": "PurchaseRequest" }`. The action is validated using the OTPAction enum.
2. Server generates OTP and delivery by logging OTP to console.
3. Client calls `POST /api/auth/validate-otp` with `{ "email":"...",
"action":"PurchaseRequest", "otp":"123456" }`.
4. Server validates OTP and returns `otpToken` (GUID). Client then calls the
protected action with header `X-OTP-Token: <otpToken>`.

## Assumptions & design decisions
- Demo uses InMemory DB for ease. Replace with persistent provider for
production.
- OTPs are short-lived -5min- and stored hashed. OTP validation returns a short-lived
token for subsequent action requests to avoid resending OTP on every call.
- JWT has a moderate expiry and contains role claims for authorization checks.
- Input validation uses DataAnnotations and controller-level checks.
- I did not understand what process sale means in this context, I thought it meant one of two, either marking a purchase request as completed or adding a sale directly for a customer, so I implemented both.

## Configuration
Check `appsettings.json` for `Jwt:Key` and `Jwt:Issuer`.

## Logging
App logs to console; OTPs are logged as simulated delivery.
