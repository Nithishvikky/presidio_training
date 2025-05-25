use Northwind;
select * from Customers;
select * from Orders;
select * from Employees;
select * from Products;
select * from Suppliers;
select * from Categories;
select * from [Order Details];

-- 1) List all orders with the customer name and the employee who handled the order.

-- (Join Orders, Customers, and Employees)

Select O.OrderId OrderId,C.ContactName CustomerName,CONCAT(E.FirstName,' ',E.LastName) EmployeeName from Orders O
join Customers C on O.CustomerID = C.CustomerID
join Employees E on O.EmployeeID = E.EmployeeID
order by CustomerName,OrderId;

-- 2) Get a list of products along with their category and supplier name.

-- (Join Products, Categories, and Suppliers)

Select P.ProductName,C.CategoryName,S.ContactName SupplierName from Products P
join Categories C on P.CategoryID = C.CategoryID
join Suppliers S on P.SupplierID = S.SupplierID
Order by P.ProductName;

-- 3) Show all orders and the products included in each order with quantity and unit price.

-- (Join Orders, Order Details, Products)

Select O.OrderId,P.ProductName,Od.Quantity,Od.UnitPrice from Orders O 
left join [Order Details] Od on O.OrderID = Od.OrderID
join Products P on P.ProductID = Od.ProductID
order by O.OrderID;

-- 4) List employees who report to other employees (manager-subordinate relationship).

-- (Self join on Employees)

select * from Employees;

select CONCAT(E.FirstName,' ',E.LastName) EmployeeName,CONCAT(Eo.FirstName,' ',Eo.LastName) ManagerName from Employees E
left join Employees Eo on E.ReportsTo = Eo.EmployeeID;


-- 5) Display each customer and their total order count.

-- (Join Customers and Orders, then GROUP BY)

Select C.ContactName Customer,Count(O.OrderId) No_of_Orders from Customers C
left join Orders O on O.CustomerID = C.CustomerID
group by C.ContactName;


-- 6) Find the average unit price of products per category.

-- Use AVG() with GROUP BY

select * from Products;
select * from Categories;

select C.CategoryName,avg(P.UnitPrice) Average from Products P
join Categories C on C.CategoryID = P.CategoryID
group by C.CategoryName;


-- 7) List customers where the contact title starts with 'Owner'.

-- Use LIKE or LEFT(ContactTitle, 5)

select ContactName,ContactTitle from Customers
where ContactTitle like '%Owner%'

-- 8) Show the top 5 most expensive products.

-- Use ORDER BY UnitPrice DESC and TOP 5

select Top 5 * from Products
order by UnitPrice desc;


-- 9) Return the total sales amount (quantity � unit price) per order.

-- Use SUM(OrderDetails.Quantity * OrderDetails.UnitPrice) and GROUP BY

select OrderID,Sum(Unitprice*Quantity) TotalAmount from [Order Details]
group by OrderID;

-- 10) Create a stored procedure that returns all orders for a given customer ID.

-- Input: @CustomerID

create proc proc_GetOrder(@Id nvarchar(30))
as
begin
	select * from orders where CustomerID = @Id
end

proc_GetOrder 'VINET'
proc_GetOrder 'TOMSP'
proc_GetOrder 'VICTE'

-- 11) Write a stored procedure that inserts a new product.

-- Inputs: ProductName, SupplierID, CategoryID, UnitPrice, etc.

select * from products;

create proc proc_InsertProducts(@ProductName nvarchar(100),@SupplierID int,@CategoryID int,@UnitPrice decimal(10,2))
as
begin
	insert into Products(ProductName,SupplierID,CategoryID,UnitPrice)
	values(@ProductName,@SupplierID,@CategoryID,@UnitPrice)
end

proc_InsertProducts 'Tea',1,1,20.00


-- 12) Create a stored procedure that returns total sales per employee.

create or alter proc proc_SalesOfEmployee
as
begin
	select E.EmployeeId,CONCAT(E.FirstName,' ',E.LastName) EmployeeName ,Count(O.OrderId) TotalSales from Employees E
	join Orders O on E.EmployeeID = O.EmployeeID
	group by E.EmployeeID,E.FirstName,E.LastName;
end

exec proc_SalesOfEmployee;

--13) Use a CTE to rank products by unit price within each category.

-- Use ROW_NUMBER() or RANK() with PARTITION BY CategoryID

with cte_rankProducts
as (select ProductId,ProductName,CategoryID,UnitPrice,row_number() over(partition by CategoryId Order by unitPrice desc) As Price_Rank from Products)
select * from cte_rankProducts;

-- 14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.

with cte_TotalRevenue AS
(select P.ProductID, SUM(Od.Quantity * Od.UnitPrice) Total_Revenue from Products P
join [Order Details] Od on P.ProductID = Od.ProductID
group by P.ProductID
having SUM(Od.Quantity * Od.UnitPrice) > 10000)
select * from cte_TotalRevenue


-- 15) Use a CTE with recursion to display employee hierarchy.

-- Start from top-level employee (ReportsTo IS NULL) and drill down
select * from Employees;

with cte_hierarchy
as
(select EmployeeId,CONCAT(FirstName,' ',LastName) EmployeeName,ReportsTo, 1 AS Hierarchy_level from Employees
where ReportsTo is null

union all

select E.EmployeeId,CONCAT(E.FirstName,' ',E.LastName) EmployeeName,E.ReportsTo,Eh.Hierarchy_level+1 from Employees E
join cte_hierarchy Eh on E.ReportsTo = Eh.EmployeeID)
select * from cte_hierarchy
order by Hierarchy_level,EmployeeId;


