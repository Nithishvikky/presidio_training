-- Design the database for a shop which sells products
-- Points for consideration
--   1) One product can be supplied by many suppliers
--   2) One supplier can supply many products
--   3) All customers details have to present
--   4) A customer can buy more than one product in every purchase
--   5) Bill for every purchase has to be stored
--   6) These are just details of one shop

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
	name varchar(20) not null,
	status boolean not null
)

create table country(
	id int primary key,
	name varchar(20) not  null
)

create table states(
	id int primary key,
	name varchar(20) not null,
	country_id int not null,
	FOREIGN KEY (country_id) REFERENCES country(id)
)

create table city(
	id int primary key,
	name varchar(20) not null,
	state_id int not null,
	FOREIGN KEY (state_id) REFERENCES states(id)
)

create table area(
	zipcode int primary key,
	name varchar(20) not null,
	city_id int not null,
	FOREIGN KEY (city_id) REFERENCES city(id)
)

create table addresses(
	id int primary key,
	door_number int not null,
	addressline1 varchar(40),
	zipcode int not null,
	FOREIGN KEY (zipcode) REFERENCES area(zipcode)
)

create table supplier(
	id int primary key,
	name varchar(20) not null,
	contact_person varchar(20),
	phone varchar(10) not null,
	email varchar(40),
	address_id int not null,
	status boolean not null,
	FOREIGN KEY (address_id) REFERENCES addresses(id)
)

create table products(
	id int primary key,
	name varchar(20) not null,
	unit_price decimal(10,2) not null,
	quantity int not null,
	description text,
	image varchar(255)
)

create table product_supplier(
	transaction_id int primary key,
	product_id int not null,
	supplier_id int not null,
	date_of_supply date not null,
	quantity int not null,
	FOREIGN KEY (product_id) REFERENCES products(id),
	FOREIGN KEY (supplier_id) REFERENCES supplier(id)
)

create table customer(
	id int primary key,
	name varchar(30) not null,
	phone varchar(30) not null,
	age int not null,
	address_id int not null,
	FOREIGN KEY (address_id) REFERENCES addresses(id),
	CHECK (age >= 18)
)

create table orders(
	order_number int primary key,
	customer_id int not null,
	product_id int not null,
	qunatity int not null,
	unit_price decimal(10,2) not null,
	FOREIGN KEY (customer_id) REFERENCES customer(id),
	FOREIGN KEY (product_id) REFERENCES products(id)
)