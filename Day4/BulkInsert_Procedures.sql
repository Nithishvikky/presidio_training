
create table people
(id int primary key,
name nvarchar(20),
age int)

create table BulkInsertLog
(LogId int identity(1,1) primary key,
FilePath nvarchar(1000),
status nvarchar(50) constraint chk_status Check(status in('Success','Failed')),
Message nvarchar(1000),
InsertedOn DateTime default GetDate())

create or alter proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
  Begin try
	   declare @insertQuery nvarchar(max)

	   set @insertQuery = 'BULK INSERT people from '''+ @filepath +'''
	   with(
	   FIRSTROW =2,
	   FIELDTERMINATOR='','',
	   ROWTERMINATOR = ''\n'')'

	   exec sp_executesql @insertQuery

	   insert into BulkInsertLog(filepath,status,message)
	   values(@filepath,'Success','Bulk insert completed')
  end try
  begin catch
		 insert into BulkInsertLog(filepath,status,message)
		 values(@filepath,'Failed',Error_Message())
  END Catch
end

proc_BulkInsert 'C:\Users\DELL\Desktop\Work\Day4\Data.csv'

select * from BulkInsertLog;
select * from people;


-------------------------------------------------------------------------------
-- In & Out Example

select * from products where 
try_cast(json_value(details,'$.spec.cpu') as nvarchar(20)) ='i5'

create or alter proc proc_FilterProducts(@pcpu varchar(20), @pcount int out)
as
begin
     set @pcount = (select count(*) from products where 
     try_cast(json_value(details,'$.spec.cpu') as nvarchar(20)) =@pcpu)
end

begin
  declare @cnt int
  exec proc_FilterProducts 'i5', @cnt out
  print concat('The number of computers is ',@cnt)
 end