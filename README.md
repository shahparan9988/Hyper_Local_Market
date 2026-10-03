# HyperLocalMarket (HLM)

**A location-focused marketplace and seller management platform built with .NET and React.**

HyperLocalMarket is designed to connect buyers with nearby local businesses and service providers based on location. It is aimed especially at small sellers and service providers who may not have their own website but still want an online presence, better visibility, and practical tools to manage their businesses.

The platform is particularly useful in densely populated areas where customers often know what they need but struggle to identify which nearby seller or service providers actually provides it. Every store on HLM is tied to a real location, making local discovery a core part of the platform.

Although HLM is focused on local commerce, customers are not limited to nearby stores. They can also search for products or services in other regions when the required item is unavailable locally or when they are willing to travel, arrange pickup, or use delivery. This means HLM can support both immediate nearby discovery and broader location-based searching across different areas.

Alongside marketplace discovery, sellers can manage their product/service catalog, variants, categories, inventory, store information, business hours, pickup and delivery options, and product images from one platform.

## Technology

- **Backend:** C#, .NET 8, ASP.NET Core, Entity Framework Core
- **Frontend:** React
- **Database:** PostgreSQL with PostGIS for spatial and location data
- **Architecture:** DDD, CQRS, MediatR, layered architecture and transactional outbox
- **AWS:** S3, SQS and CloudFront with a dedicated .NET background worker for asynchronous image processing
- **Engineering concepts:** REST APIs, authentication and authorization, optimistic concurrency, idempotency and background processing

## Completed so far

- User registration, login, session-based authentication and access control
- Seller store creation, setup and publishing
- Business information, contact details, location, business hours and store branding
- Product and service catalog management
- Product variants, prices, SKUs and unlimited-depth seller categories
- Product and store image uploads through S3 with asynchronous SQS/worker processing
- Pickup and configurable delivery options
- Optional inventory tracking at product-variant level
- Inventory overview with search, filters, stock status and pagination
- PostgreSQL/PostGIS spatial storage and indexing as the foundation for location-based discovery

## In progress / planned

- Full stock movement history and inventory audit trail
- Purchasing and supplier management
- Order processing and stock reservations
- Walk-in and external sales-channel recording
- Expenses, cost tracking and sales/profit reporting
- Public buyer storefronts
- Nearby store and product/service discovery using location
- Region-based product and service search beyond the buyer's immediate area
- Buyer-seller messaging
- Order fulfillment and returns
- Future delivery/rider network
- Most importantly an AI-Manager implementation in future for every seller and buyer

HLM is an ongoing project focused on combining a **location-based marketplace** with the operational tools small sellers need to run and grow their business.
