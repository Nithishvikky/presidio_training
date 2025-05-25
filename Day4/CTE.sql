
with cte_product
as
(select id,name from products)
select * from cte_product

declare @page int =2, @pageSize int=10;
with PaginatedBooks as
( select  title_id,title, price, ROW_Number() over (order by price desc) as RowNum from titles)
select * from PaginatedBooks where rowNUm between((@page-1)*@pageSize) and (@page*@pageSize)

--create a sp that will take the page number and size as param and print the books
create or alter proc proc_PaginateTitles( @page int =1, @pageSize int=10)
as
begin
with PaginatedBooks as
( select  title_id,title, price, ROW_Number() over (order by price desc) as RowNum
  from titles
)
select * from PaginatedBooks where RowNum between((@page-1)*(@pageSize+1)) and (@page*@pageSize)
end

exec proc_PaginateTitles 2,5

select  title_id,title, price from titles
order by price desc
offset 10 rows fetch next 10 rows only