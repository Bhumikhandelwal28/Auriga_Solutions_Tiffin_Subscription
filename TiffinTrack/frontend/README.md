# TiffinTrack – Tiffin Subscription Management System

TiffinTrack is a full-stack application for managing a home-style weekday tiffin subscription service.

The system helps the owner manage customers, subscription plans, pauses, deliveries and monthly billing.

## Features

* Customer registration and login
* Customer search using name or phone number
* Customer subscription management
* Tiffin plan management
* Pause and resume subscriptions
* Cancel subscriptions
* Weekday-based delivery processing
* Delivery notification outbox
* Monthly bill calculation based on actual delivered days
* Responsive owner dashboard
* CSV customer import interface

## Technology Stack

### Backend

* ASP.NET Core Web API
* .NET 10
* Entity Framework Core
* SQLite
* BCrypt.Net
* Swagger / OpenAPI

### Frontend

* React
* Vite
* Axios
* React Router
* CSS

## Project Structure

```text
TiffinTrack/
├── backend/
│   └── TiffinTrack.API/
│       ├── Controllers/
│       ├── DTOs/
│       ├── Data/
│       ├── Models/
│       ├── Services/
│       ├── Migrations/
│       └── Program.cs
│
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── api.js
│   │   ├── App.jsx
│   │   └── App.css
│   └── package.json
│
├── README.md
├── REASONING.md
└── AI_LOGS.md
```

# Setup

## Prerequisites

Install:

* .NET 10 SDK
* Node.js
* npm
* Git

## Backend Setup

Open a terminal and move to the backend:

```bash
cd backend/TiffinTrack.API
```

Restore dependencies:

```bash
dotnet restore
```

Apply the database migrations:

```bash
dotnet ef database update
```

Run the backend:

```bash
dotnet run
```

The backend runs on:

```text
http://localhost:5088
```

Swagger is available at:

```text
http://localhost:5088/swagger
```

When using GitHub Codespaces, the forwarded port URL can be used by the frontend instead of localhost.

## Frontend Setup

Open another terminal:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

The Vite development server normally runs on:

```text
http://localhost:5173
```

## Frontend API Configuration

The backend URL is configured in:

```text
frontend/src/api.js
```

Example:

```javascript
import axios from "axios";

const api = axios.create({
  baseURL: "https://YOUR-BACKEND-FORWARDED-URL.app.github.dev/api"
});

export default api;
```

In GitHub Codespaces, the forwarded backend URL should be used when the browser cannot access `localhost:5088`.

# Default Demo Account

The application seeds a demo user when the Users table is empty.

```text
Phone: 9999999999
Password: Password@123
```

A new account can also be created using the registration API or registration page.

# API Endpoints

## Authentication

### Register

```http
POST /api/auth/register
```

Example request:

```json
{
  "name": "Test User",
  "phone": "9876543210",
  "password": "Password@123"
}
```

### Login

```http
POST /api/auth/login
```

Example request:

```json
{
  "phone": "9876543210",
  "password": "Password@123"
}
```

## Customers

### Get Customers

```http
GET /api/customers
```

Supports:

* Search
* Pagination
* Sorting

Example:

```http
GET /api/customers?search=9876&page=1&pageSize=10&sortBy=name&sortOrder=asc
```

### Get Customer Details

```http
GET /api/customers/{id}
```

### Get Customer Subscriptions

```http
GET /api/customers/{id}/subscriptions
```

## Plans

### Get Active Plans

```http
GET /api/plans
```

### Get Plan

```http
GET /api/plans/{id}
```

### Create Plan

```http
POST /api/plans
```

Example:

```json
{
  "name": "Standard Lunch",
  "description": "Homestyle weekday lunch",
  "monthlyPrice": 2500
}
```

### Update Plan

```http
PUT /api/plans/{id}
```

### Deactivate Plan

```http
DELETE /api/plans/{id}
```

The delete operation uses soft deletion by setting the plan as inactive.

## Subscriptions

### Create Subscription

```http
POST /api/subscriptions
```

Example:

```json
{
  "userId": 1,
  "planId": 1,
  "startDate": "2026-09-01"
}
```

### Get Subscription

```http
GET /api/subscriptions/{id}
```

### Pause Subscription

```http
POST /api/subscriptions/{id}/pause
```

Example:

```json
{
  "startDate": "2026-09-15",
  "endDate": "2026-09-18",
  "reason": "Travel"
}
```

### Resume Subscription

```http
POST /api/subscriptions/{id}/resume
```

### Cancel Subscription

```http
POST /api/subscriptions/{id}/cancel
```

## Billing

### Calculate Monthly Bill

```http
GET /api/billing/{customerId}?year=2026&month=9
```

The billing calculation:

1. Finds eligible weekdays from the subscription start date.
2. Removes weekdays covered by pause periods.
3. Calculates the daily rate from the monthly plan price.
4. Multiplies the daily rate by the actual delivered days.

## Daily Delivery Clock

### Run Daily Delivery Processing

```http
POST /api/clock
```

Optional date:

```http
POST /api/clock?date=2026-09-17
```

The clock:

* Processes weekdays only.
* Finds active subscriptions.
* Skips customers whose subscription is paused for that date.
* Creates a delivery notification for eligible customers.

## Notification Outbox

### Get Notifications

```http
GET /api/outbox
```

The outbox contains the delivery notifications generated by the daily clock.

# Debugging

## Backend does not start

Run:

```bash
dotnet build
```

If there are package or dependency issues:

```bash
dotnet restore
```

Then:

```bash
dotnet run
```

## Database issues

Apply migrations again:

```bash
dotnet ef database update
```

## Frontend cannot connect to backend

Check the API URL in:

```text
frontend/src/api.js
```

In GitHub Codespaces, make sure the backend port is forwarded and use its forwarded URL.

For example:

```javascript
const api = axios.create({
  baseURL: "https://YOUR-BACKEND-URL.app.github.dev/api"
});
```

## 404 error during login

The backend login route is:

```text
/api/auth/login
```

Therefore the Axios base URL must end with:

```text
/api
```

For example:

```text
https://YOUR-BACKEND-URL.app.github.dev/api
```

If `/api` is missing, the frontend may call:

```text
/auth/login
```

instead of:

```text
/api/auth/login
```

## Testing

Swagger can be used to test backend APIs independently.

Open:

```text
http://localhost:5088/swagger
```

This was also useful for separating frontend connection issues from backend API issues.

# Running the Complete Project

Start the backend:

```bash
cd backend/TiffinTrack.API
dotnet run
```

Start the frontend in another terminal:

```bash
cd frontend
npm run dev
```

Then open the Vite frontend URL in the browser.

# Git

To commit changes:

```bash
git add .
git commit -m "Complete TiffinTrack implementation"
git push
```
