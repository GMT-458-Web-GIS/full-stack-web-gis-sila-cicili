Link: https://www.youtube.com/watch?v=knZImprxyPM
# 📚 Web GIS Library Management System

![.NET Core](https://img.shields.io/badge/.NET%20Core-7.0-purple)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-blue)
![PostGIS](https://img.shields.io/badge/PostGIS-Enabled-green)


This project was developed as a final assignment for the **GMT 458 – Web GIS** course. It is a web-based **Library Information System** capable of managing both spatial and non-spatial data, designed for users with different roles.

The project is designed with a modern architecture using **ASP.NET Core MVC**, **PostgreSQL (PostGIS)**, and **Entity Framework Core**.

---

## 🚀 Project Features and Requirements Rubric

The table below summarizes to what extent the project requirements have been met:

| Requirement | Status | Description |
| :--- | :---: | :--- |
| **Source Code Management** | ✅ Done | Project version control is provided via GitHub. |
| **Managing User Types** | ✅ Done | **3 Different Roles:** <br>🎓 **Student:** 15-day borrowing period.<br>👨‍🏫 **Academician:** 30-day borrowing period.<br>🛡️ **Admin:** Full authorization. |
| **CRUD Operations (Spatial)** | ✅ Done | Library branches (Spatial Point) can be **Added, Deleted, Updated, and Listed** via the map. |
| **Authentication** | ✅ Done | Secure cookie-based **Sign-up** and **Login** mechanism. |
| **API Development** | ✅ Done | **RESTful API:** Spatial (Branch) and Non-spatial (Book) data are exposed. <br>📄 **Swagger:** Documentation is available at `/swagger`. |
| **Database** | ✅ Done | **PostgreSQL** is used for relational data, and **PostGIS** is used for geographic data. |
| **Dashboard** | ✅ Done | The admin panel includes real-time statistics and **Chart.js** graphs showing book categories. |
| **Performance Testing** | ✅ Done | Load and Stress tests were applied using **Apache JMeter**, and response times were analyzed. |
| **Performance Monitoring** | ✅ Done | The effect of **B-Tree** and **R-Tree** indexing on query performance was analyzed. |

---

## 🛠️ Tech Stack

* **Backend:** ASP.NET Core 7.0 (MVC & Web API)
* **Database:** PostgreSQL 14+ & PostGIS Extension
* **ORM:** Entity Framework Core (Spatial data support with NetTopologySuite)
* **Frontend:** HTML5, Bootstrap 5, JavaScript
* **Visualization:** Chart.js (Statistics), Leaflet/Google Maps (Map Interface)
* **Test & Documentation:** Apache JMeter, Swagger UI

---
![Intro Video](images/Ekrangörüntüsü2025-12-29191223.png)

---

## 📸 Screenshots

### 1. Management Panel (Dashboard)
Summary statistics and graphical reports for administrators.
![Panel](images/kullanıcı.png)

### 2. Swagger API Documentation
Interface for testing RESTful services.
![Swagger](images/swagger.png)

### 3. Map and Branch Management
PostGIS-supported branch addition and viewing screen.
![Map](images/harita.png)

---

## ⚙️ Installation

Follow the steps below to run the project on your local machine:

1.  **Clone the Project:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/LibrarySystem.git](https://github.com/YOUR_USERNAME/LibrarySystem.git)
    cd LibrarySystem
    ```

2.  **Configure Database Connection:**
    Open the `appsettings.json` file and edit the `ConnectionStrings` section according to your own PostgreSQL credentials:
    ```json
    "ConnectionStrings": {
      "LibraryContext": "Host=localhost;Database=LibraryDb;Username=postgres;Password=your_password"
    }
    ```

3.  **Create the Database (Migration):**
    Open the terminal in the project directory and run the following command:
    ```bash
    dotnet ef database update
    ```

4.  **Start the Project:**
    ```bash
    dotnet run
    ```
---

## 🔗 API Usage

To test API endpoints while the project is running:
👉 **URL:** `https://localhost:7239/swagger`

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| **GET** | `/api/LibraryApi/branches` | Retrieves all library branches (GeoJSON). |
| **POST** | `/api/LibraryApi/branches` | Adds a new branch. |
| **PUT** | `/api/LibraryApi/branches/{id}` | Updates branch information. |
| **DELETE** | `/api/LibraryApi/branches/{id}` | Deletes the branch. |

---

## 🚀 Performance and Load Testing
This stress test was conducted to measure the stability of the application under loads far exceeding normal usage limits (Peak Traffic). The goal is to analyze whether the database connection pool clogs, if the API crashes, and if data integrity is maintained when 600 concurrent users suddenly access the system. This test aims to confirm that the system is not only fast but also sustainable and resilient under challenging conditions.

---
**Apache JMeter** was used to measure system resilience. **50,000 dummy book records** were added to the database, and tests were run on this dataset.

### 📊 Test Results

| Test Type | Users (Threads) | Purpose | Avg Response Time | Result |
| :--- | :---: | :--- | :---: | :--- |
| **Load Test** | 100 | Simulation of normal usage | **34 ms** | ✅ Successful |
| **Stress Test** | 600 | Pushing the system to limits | **3400 ms** | ✅ Stable |

#### 1. Load Test (100 Users)
![Load Test Graph](images/100.png)

#### 2. Stress Test (600 Users)
![Stress Test Graph](images/1000.png)
---
As a result of the 1000-user stress test, although the system responded above the normal operating time (3.4 sec), it provided uninterrupted accessibility (100% Availability). The absence of any HTTP 500 errors or system crashes indicates that the infrastructure is robust enough to tolerate traffic spikes.

---

## ⚡ Database Indexing Experiment (Performance Monitoring)

An experiment was conducted using the PostgreSQL `EXPLAIN ANALYZE` command to observe the effect of database indexing (B-Tree) on query performance.

* **Scenario:** Searching for a specific book by the `title` column.
* **Dataset:** 50,000 Rows.
* **Query:**
    ```sql
    SELECT * FROM "books" WHERE "title" = 'Performans Test Kitabı 45000';
    ```

### 🧪 Results and Comparison

| Metric | Pre-Index (Sequential Scan) | Post-Index (B-Tree Index Scan) | Improvement |
| :--- | :--- | :--- | :---: |
| **Scan Type** | Reads all rows (Seq Scan) | Goes directly to address (Index Scan) | - |
| **Query Time** | **22.742 ms** | **0.100 ms** | **~%99** 🚀 |
| **Planning Time**| 2.294 ms | 4.961 ms | - |

#### 1. Pre-Index (Sequential Scan)
Since there was no index, the database had to check all 50,000 rows one by one.
![Sequential Scan](images/indexsiz.png)

#### 2. Post-Index (B-Tree Optimized)
After adding a B-Tree index to the `title` column, the data was found instantly.
![Index Scan](images/indexli1.png)

---
*This project was prepared by Sıla CİCİLİ for the GMT 458 course.*
