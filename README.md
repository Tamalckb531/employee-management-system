# employee-management-system
Assessment project for Fionetix Solution

# Docker command :
docker pull postgres:15

docker run -d \
  --name employee_postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=employee_management \
  -p 5432:5432 \
  postgres:15

**For_Migration**

cd server
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run   # seeds database automatically

**To_See_Database**
docker exec -it employee_postgres psql -U postgres -d employee_management

\dt                  -- list all tables
SELECT * FROM "Admins";
SELECT * FROM "Employees";
SELECT * FROM "Spouses";
SELECT * FROM "Children";
\q                   -- quit