-- categories
--  id, name, status
 
-- country
--  id, name
 
-- state
--  id, name, country_id
 
-- City
--  id, name, state_id
 
-- area
--  zipcode, name, city_id
 
-- address
--  id, door_number, addressline1, zipcode
 
-- supplier
--  id, name, contact_person, phone, email, address_id, status
 
-- product
--  id, Name, unit_price, quantity, description, image
 
-- product_supplier
--  transaction_id, product_id, supplier_id, date_of_supply, quantity,
 
-- Customer
--  id, Name, Phone, age, address_id
 
-- order
--  order_number, customer_id, Date_of_order, amount, order_status
 
-- order_details
--  id, order_number, product_id, quantity, unit_price
--------------------------------------------------------------------------

-- DDL (Creation of each tables)

create table category(
	id int primary key,
	name varchar(20),
	status varchar(50)
)

create table country(
	id int primary key,
	name varchar(20)
)

create table states(
	id int primary key,
	name varchar(20),
	country_id int,
	FOREIGN KEY (country_id) REFERENCES country(id)
)

create table city(
	id int primary key,
	name varchar(20),
	state_id int,
	FOREIGN KEY (state_id) REFERENCES states(id)
)

create table area(
	zipcode int primary key,
	name varchar(20),
	city_id int,
	FOREIGN KEY (city_id) REFERENCES city(id)
)

create table addresses(
	id int primary key,
	door_number int,
	addressline1 varchar(40),
	zipcode int,
	FOREIGN KEY (zipcode) REFERENCES area(zipcode)
)

create table supplier(
	id int primary key,
	name varchar(20),
	contact_person varchar(20),
	phone varchar(10),
	email varchar(40),
	address_id int,
	status varchar(20),
	FOREIGN KEY (address_id) REFERENCES addresses(id)
)

create table products(
	id int primary key,
	name varchar(20),
	unit_price decimal(10,2),
	quantity int,
	description text,
	image blob
)

create table product_supplier(
	transaction_id int primary key,
	product_id int,
	supplier_id int,
	date_of_supply date default current_date,
	quantity int,
	FOREIGN KEY (product_id) REFERENCES products(id),
	FOREIGN KEY (supplier_id) REFERENCES supplier(id)
)

create table customer(
	id int primary key,
	name varchar(30) not null,
	phone varchar(30) not null,
	age int not null,
	address_id int,
	FOREIGN KEY (address_id) REFERENCES addresses(id),
	CHECK (age >= 18)
)

create table orders(
	order_number int primary key,
	customer_id int,
	product_id int,
	qunatity int,
	unit_price decimal(10,2),
	FOREIGN KEY (customer_id) REFERENCES customer(id),
	FOREIGN KEY (product_id) REFERENCES products(id)
)