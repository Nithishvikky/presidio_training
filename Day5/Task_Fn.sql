
-- SELECT Queries
-- List all films with their length and rental rate, sorted by length descending.
-- Columns: title, length, rental_rate
select title,length,rental_rate from film;

-- Find the top 5 customers who have rented the most films.

select concat(C.first_name,' ',C.last_name) CustomerName,Count(R.rental_id) FlimRented from customer C
join rental R on R.customer_id = C.customer_id
group by CustomerName
order by FlimRented desc
limit 5;


-- Display all films that have never been rented.
-- Hint: Use LEFT JOIN between film and inventory → rental.

select F.film_id,F.title from film F
left join inventory I on I.film_id=F.film_id
left join rental R on R.inventory_id = I.inventory_id
where R.rental_id is null;

-- JOIN Queries
-- List all actors who appeared in the film ‘Academy Dinosaur’.
-- Tables: film, film_actor, actor

select fa.actor_id,concat(a.first_name,' ',a.last_name) ActorName,f.title from film_actor fa
join film f on f.film_id=fa.film_id
left join actor a on a.actor_id = fa.actor_id
where f.title = 'Academy Dinosaur';

-- List each customer along with the total number of rentals they made and the total amount paid.
-- Tables: customer, rental, payment

select concat(C.first_name,' ',C.last_name) CustomerName,count(R.rental_id) TotalRental,sum(P.amount) TotalAmount from customer C
left join rental R on R.customer_id = C.customer_id
join payment P on P.customer_id = C.customer_id
group by CustomerName
order by CustomerName;

-- CTE-Based Queries
-- Using a CTE, show the top 3 rented movies by number of rentals.
-- Columns: title, rental_count

with TopRentedMovies
as
(select F.title FilmName,count(R.rental_id) Rentals from inventory I
join film F on F.film_id = I.film_id
join rental R on I.inventory_id = R.inventory_id
group by FilmName
order by 2 desc)
select * from TopRentedMovies 
limit 3;

-- Find customers who have rented more than the average number of films.
-- Use a CTE to compute the average rentals per customer, then filter.

with cte_AvgRentalsPerCustomer as
(
 select avg(Rental_Count) AS Avg_Rental_Count from
	( 
		select customer_id, count(*) as Rental_Count from rental 
		group by customer_id
	) as Avg_Rental_Count
)
select c.customer_id, concat(first_name,' ',last_name) Customer_Name,count(*) AS Total_Rentals 
from customer c
join rental r ON c.customer_id = r.customer_id
group by c.customer_id
having count(*) > (select Avg_Rental_Count from cte_AvgRentalsPerCustomer); 

--  Function Questions
-- Write a function that returns the total number of rentals for a given customer ID.
-- Function: get_total_rentals(customer_id INT)

 CREATE OR REPLACE FUNCTION get_total_rentals(cust_id INT) 
 RETURNS INT AS 
 $$ 
 DECLARE total_rentals INT; 
 BEGIN 
	 SELECT COUNT(*) INTO total_rentals FROM rental WHERE customer_id = cust_id; 
	 RETURN total_rentals; 
 END; 
 $$ 
 LANGUAGE plpgsql

 select get_total_rentals(2);

-- Stored Procedure Questions
-- Write a stored procedure that updates the rental rate of a film by film ID and new rate.
-- Procedure: update_rental_rate(film_id INT, new_rate NUMERIC)
create procedure proc_updateRantal(filmId int,newRate numeric(4,2))
LANGUAGE plpgsql
as $$
begin
	update film set rental_rate = newRate
	where film_id = filmId;
end;
$$;

call proc_updateRantal(2,5.00)

-- Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).
-- Procedure: get_overdue_rentals() that selects relevant columns.

select * from rental;

create procedure get_overdue_rentals()
LANGUAGE plpgsql
as $$
begin
	select * from rental
	where return_date is null or return_date >= rental_date + interval '7 days';
end;
$$;

drop procedure get_overdue_rentals()
call get_overdue_rentals()