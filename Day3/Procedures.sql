create table Products(
    id int identity(1,1) constraint pk_productId primary key,
	name nvarchar(100) not null,
	details nvarchar(max)
)
	
-- details in Json
-- Stored procedure for Insertion of data

create proc proc_InsertProduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
    insert into Products(name,details) values(@pname,@pdetails)
end


proc_InsertProduct 'Mobile','{"brand":"Iqoo","spec":{"ram":"16GB","cpu":"snapdragon"}}';

select * from Products

-- To extract and select the json type data

select JSON_QUERY(details,'$.spec') spec from products;
select JSON_VALUE(details,'$.brand') brand from products;

-- Stored procedure for Updation of data

create proc proc_UpdateProductSpec(@pid int,@newvalue varchar(20))
as
begin
   	update products set details = JSON_MODIFY(details, '$.spec.ram',@newvalue) where id = @pid
end

proc_UpdateProductSpec 2, '24GB'

-- Try_Cast used for preventing from getting error
select * from products where 
try_cast(json_value(details,'$.spec.cpu') as nvarchar(20)) = 'i5'

-- Create a procedure that brings post by taking the user_id as parameter

create proc proc_GetPostbyUserId(@value int)
as
begin
		select * from posts 
		where user_id = @value
end
	
proc_GetPostbyUserId 2