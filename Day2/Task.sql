
create database Tasks;

use Tasks;


create table Items(
	Itemname varchar(50) primary key,
	Itemtype varchar(1),
	Itemcolor varchar(15)
)

create table Department(
	Deptname varchar(20) primary key,
	Deptfloor int,
	Deptphone int,
	ManagerId int not null,
)

create table Emp(
	Empno int primary key,
	Empname varchar(20),
	Empsalary int,
	Department varchar(20) null,
	Bossno int null,
	FOREIGN KEY (Department) REFERENCES Department(Deptname),
	FOREIGN KEY (Bossno) REFERENCES Emp(Empno)
)

create table Sale(
	Salesno int primary key,
	Saleqty int,
	Itemname varchar(50) Not null,
	Deptname varchar(20) not null,
	FOREIGN KEY (Itemname) REFERENCES Items(Itemname),
	FOREIGN KEY (Deptname) REFERENCES Department(Deptname)
)

alter table Department
add foreign key (ManagerId) references Emp(Empno);


INSERT INTO Emp (Empno, Empname, Empsalary, Bossno) VALUES
(1, 'Alice', 75000,NULL),
(2, 'Ned', 45000,1),
(3, 'Andrew', 25000,2),
(4, 'Clare', 22000,2),
(5, 'Todd', 38000,1),
(6, 'Nancy', 22000,5),
(7, 'Brier', 43000,1),
(8, 'Sarah', 56000,7),
(9, 'Sophile', 35000,1),
(10, 'Sanjay', 15000, 3),
(11, 'Rita', 15000,4),
(12, 'Gigi', 16000,4),
(13, 'Maggie', 11000,4),
(14, 'Paul', 15000, 3),
(15, 'James', 15000,3),
(16, 'Pat', 15000,3),
(17, 'Mark', 15000, 3);


select * from emp;

INSERT INTO Department (Deptname, Deptfloor, Deptphone, ManagerId) VALUES
('Management', 5, 34, 1),
('Books', 1, 81, 4),
('Clothes', 2, 24, 4),
('Equipment', 3, 57, 3),
('Furniture', 4, 14, 3),
('Navigation', 1, 41, 3),
('Recreation', 2, 29, 4),
('Accounting', 5, 35, 5),
('Purchasing', 5, 36, 7),
('Personnel', 5, 37, 9),
('Marketing', 5, 38, 2);

select * from department;

UPDATE Emp SET Department = 'Management' WHERE Empno IN (1);
UPDATE Emp SET Department = 'Marketing' WHERE Empno IN (2, 3, 4);
UPDATE Emp SET Department = 'Accounting' WHERE Empno IN (5, 6);
UPDATE Emp SET Department = 'Purchasing' WHERE Empno IN (7, 8);
UPDATE Emp SET Department = 'Personnel' WHERE Empno IN (9);
UPDATE Emp SET Department = 'Navigation' WHERE Empno IN (10);
UPDATE Emp SET Department = 'Books' WHERE Empno IN (11);
UPDATE Emp SET Department = 'Clothes' WHERE Empno IN (12, 13);
UPDATE Emp SET Department = 'Equipment' WHERE Empno IN (14, 15);
UPDATE Emp SET Department = 'Furniture' WHERE Empno IN (16);
UPDATE Emp SET Department = 'Recreation' WHERE Empno IN (17);


select * from emp;

INSERT INTO Items (Itemname, Itemtype, Itemcolor) VALUES
('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', 'E', 'Brown'),
('Compass', 'N', NULL),
('Geo positioning system', 'N', NULL),
('Elephant Polo stick', 'R', 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', NULL),
('Map Measure', 'N', NULL),
('Boots-snake proof', 'C', 'Green'),
('Pith Helmet', 'C', 'Khaki'),
('Hat-polar Explorer', 'C', 'White'),
('Exploring in 10 Easy Lessons', 'B', NULL),
('Hammock', 'F', 'Khaki'),
('How to win Foreign Friends', 'B', NULL),
('Map case', 'E', 'Brown'),
('Safari Chair', 'F', 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black'),
('Tent - 2 person', 'F', 'Khaki'),
('Tent - 8 person', 'F', 'Khaki');

select * from items;

INSERT INTO Sale (Salesno, Saleqty, Itemname, Deptname) VALUES
(101, 2, 'Boots-snake proof', 'Clothes'),
(102, 1, 'Pith Helmet', 'Clothes'),
(103, 1, 'Sextant', 'Navigation'),
(104, 3, 'Hat-polar Explorer', 'Clothes'),
(105, 5, 'Pith Helmet', 'Equipment'),
(106, 2, 'Pocket Knife-Nile', 'Clothes'),
(107, 3, 'Pocket Knife-Nile', 'Recreation'),
(108, 1, 'Compass', 'Navigation'),
(109, 2, 'Geo positioning system', 'Navigation'),
(110, 5, 'Map Measure', 'Navigation'),
(111, 1, 'Geo positioning system', 'Books'),
(112, 1, 'Sextant', 'Books'),
(113, 3, 'Pocket Knife-Nile', 'Books'),
(114, 1, 'Pocket Knife-Nile', 'Navigation'),
(115, 1, 'Pocket Knife-Nile', 'Equipment'),
(116, 1, 'Sextant', 'Clothes'),
(117, 1, 'Sextant', 'Equipment'),
(118, 1, 'Sextant', 'Recreation'),
(119, 1, 'Sextant', 'Furniture'),
(120, 1, 'Pocket Knife-Nile', 'Furniture'),
(121, 1, 'Exploring in 10 easy lessons', 'Books'),
(122, 1, 'How to win foreign friends', 'Books'),
(123, 1, 'Compass', 'Books'),
(124, 1, 'Pith Helmet', 'Books'),
(125, 1, 'Elephant Polo stick', 'Recreation'),
(126, 1, 'Camel Saddle', 'Recreation');

select * from sale;


select * from emp;
select * from department;
select * from items;
select * from sale;