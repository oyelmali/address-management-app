#  Address Management Application

A full-stack web application for managing Nova Poshta branch addresses with bulk import functionality. Built with .NET 8 backend and Next.js frontend, deployed on Google Cloud infrastructure.

##  Live Demo

- **Frontend Application**: [https://address-management-app.web.app/](https://address-management-app.web.app/)
- **Backend API**: [https://addressapp-api-1012108375831.europe-west1.run.app/](https://addressapp-api-1012108375831.europe-west1.run.app/)
- **API Documentation (Swagger)**: [https://addressapp-api-1012108375831.europe-west1.run.app/swagger](https://addressapp-api-1012108375831.europe-west1.run.app/swagger)


##  Features

### Backend
- ✅ **CRUD Operations**: Complete Create, Read, Update, Delete functionality for addresses
- ✅ **Bulk Import**: Parse and import multiple addresses from formatted text
- ✅ **Smart Text Parsing**: Automatically extracts region, city, branch type, branch number, street address, phone, and working hours from Ukrainian format
- ✅ **Search & Filter**: Search across all address fields
- ✅ **RESTful API**: Well-structured REST endpoints with proper HTTP methods
- ✅ **Data Validation**: Input validation using FluentValidation
- ✅ **Error Handling**: Comprehensive error handling with meaningful messages
- ✅ **Logging**: Structured logging with Serilog
- ✅ **Health Checks**: Monitoring endpoint for application health
- ✅ **API Documentation**: Interactive Swagger/OpenAPI documentation

### Frontend
- ✅ **Responsive Design**: Optimized for both desktop and mobile devices
- ✅ **Desktop View**: Table layout with sortable columns
- ✅ **Mobile View**: Custom card-based layout for better readability
- ✅ **Real-time Search**: Client-side filtering with debounced API search
- ✅ **CRUD Interface**: User-friendly forms for adding and editing addresses
- ✅ **Bulk Import UI**: Modal interface for bulk importing addresses
- ✅ **Visual Feedback**: Toast notifications for user actions
- ✅ **Loading States**: Smooth loading indicators
- ✅ **Error Handling**: User-friendly error messages

##  Technology Stack

### Backend
- **.NET 8**: Latest LTS version of .NET
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 8**: ORM for database operations
- **PostgreSQL**: Relational database
- **Supabase**: Managed PostgreSQL hosting
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation library
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **Docker**: Containerization

### Frontend
- **Next.js 15**: React framework with App Router
- **TypeScript**: Type-safe JavaScript
- **Tailwind CSS**: Utility-first CSS framework
- **Axios**: HTTP client
- **Lucide React**: Icon library
- **React Hooks**: Modern React patterns

### Infrastructure
- **Google Cloud Run**: Backend hosting (serverless containers)
- **Firebase Hosting**: Frontend hosting
- **Supabase**: Database hosting
- **Docker**: Container orchestration

##  Architecture

### Backend - Clean Architecture
```bash
src/
├── AddressApp.API/ # Presentation Layer
│ ├── Controllers/ # API Controllers
│ └── Program.cs # Application entry point
│
├── AddressApp.Application/ # Application Layer
│ ├── DTOs/ # Data Transfer Objects
│ ├── Services/ # Business logic
│ ├── Validators/ # Input validation
│ └── Mappings/ # AutoMapper profiles
│
├── AddressApp.Domain/ # Domain Layer
│ ├── Entities/ # Domain models
│ └── Interfaces/ # Repository contracts
│
└── AddressApp.Infrastructure/ # Infrastructure Layer
├── Data/ # Database context
└── Repositories/ # Data access implementation
**Key Principles:**
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Separation of Concerns**: Each layer has a specific responsibility
- **Repository Pattern**: Abstraction over data access
- **SOLID Principles**: Maintainable and testable code
```

### Frontend - Component-Based Architecture
```bash
frontend/
├── src/
│ ├── app/ # Next.js App Router
│ │ ├── page.tsx # Main page
│ │ └── layout.tsx # Root layout
│ │
│ ├── components/ # React components
│ │ ├── SearchBar.tsx
│ │ ├── AddressList.tsx # Desktop table view
│ │ ├── AddressCards.tsx # Mobile card view
│ │ ├── AddressForm.tsx
│ │ ├── BulkImportModal.tsx
│ │ ├── Loading.tsx
│ │ └── Toast.tsx
│ │
│ ├── services/ # API services
│ │ └── addressService.ts
│ │
│ └── types/ # TypeScript types
│ └── address.ts
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+**: [Download](https://nodejs.org/)
- **PostgreSQL** (or Supabase account)
- **Git**

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/oyelmali/address-management-app.git
   cd address-management-app
   ```
2. **Configure database connection**

uptade src/AddressApp.API/appsettings.json
   ```bash
   {
      "ConnectionStrings": {
        "DefaultConnection": "User Id=your_user;Password=your_password;Server=your_server;Port=5432;Database=your_database"
      }
    }
   ```
3. **Restore dependencies**
    ```bash
    dotnet restore
   ```
4. **Run database migrations**
   ```bash
   dotnet ef migrations add InitialCreate --project src/AddressApp.Infrastructure --startup-project src/AddressApp.API
   dotnet ef database update --project src/AddressApp.Infrastructure --startup-project src/AddressApp.API
   ```
5. **Run the backend**
   ```bash
   cd src/AddressApp.API
   dotnet run
   ```

## Backend will be available at:

* HTTPS: https://localhost:7xxx
* HTTP: http://localhost:5xxx

(Port numbers will be displayed in console)

## Access API Documentation

Open browser and navigate to:

* Swagger UI: https://localhost:xxxx/swagger
* Health Check: https://localhost:xxxx/health

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```
2. **Install dependencies**

   ```bash
   npm install
   ```
3. **Configure environment variables**

Create .env.local file in the frontend directory:
  ```bash
  NEXT_PUBLIC_API_URL=http://localhost:5xxx/api
  ```
Replace 5xxx with your actual backend port number.

4. **Run the frontend**
   ```bash
   npm run dev
   ```

Frontend will be available at: http://localhost:3000
   
5. **Access the application**
   
Open browser: http://localhost:3000

