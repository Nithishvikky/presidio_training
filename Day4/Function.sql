-- Scalar value function
create function  fn_CalculateTax(@baseprice float, @tax float)
returns float
as
begin
    return (@baseprice +(@baseprice*@tax/100))
end

select dbo.fn_CalculateTax(1000,10)
select title,dbo.fn_CalculateTax(price,12) from titles

-- Table value function
create function fn_tableSample(@minprice float)
returns table
as
    return select title,price from titles where price>= @minprice


select * from dbo.fn_tableSample(10)