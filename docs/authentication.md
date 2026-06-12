\# Authentication



\## Current Decision



Use Session-based Authentication.



\## Why



More Secure for browser-based Applications than storing JWT in frontend javascript.





\## Main Components



* User
* Session
* Session Cookie
* SessionRepository
* SessionMiddleware
* CSRF Protection



\## Flow



Login:

User submits email and password.

Backend validates user.

Backend creates session.

Backend Stores session in database.

Backend sends Httponly cookie.





Request:

Browser sends cookie automatically.

Backend validates session.

Backend identifies current user.













