-- Cursor
-- 1. Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
create table customer_rental_log (
    customer_id int,
    customer_name text,
    rental int
);
do $$
declare
    rec record;
    cur cursor for
        select count(r.rental_id) as rental,c.customer_id,c.first_name|| ' ' ||c.last_name as customer_name
        from rental r
        right join customer c on r.customer_id = c.customer_id
        group by c.customer_id;
begin
    open cur;

    loop
        fetch cur into rec;
        exit when not found;

        insert into customer_rental_log (customer_id, customer_name, rental)
        values (rec.customer_id,rec.customer_name,rec.rental);
    end loop;

    close cur;
end;
$$;
select * from customer_rental_log order by customer_id;

-- 2. Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
do $$
declare
    rec record;
    cur cursor for 
		select f.film_id,f.title from film f
		join film_category fc on fc.film_id = f.film_id
		join category c on c.category_id = fc.category_id
		where c.name = 'Comedy';
	rental_count int;
	cnt int :=0;  
begin
    open cur;

    loop
        fetch cur into rec;
        exit when not found;
		
		select count(*) into rental_count from rental r
		join inventory i on i.inventory_id = r.inventory_id
		where i.film_id = rec.film_id;

		if rental_count > 10 then
        	raise notice 'film_id : % , title : %, count : %',
					rec.film_id,rec.title,rental_count;
			cnt := cnt +1;
		end if;
    end loop;
		raise notice 'Rows returned : %',cnt;
    close cur;
end;
$$;

-- 3.Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
create table films_in_store(
    store_id int,
    number_of_films int
);

select * from films_in_store;

do $$
declare
    rec record;
    cur cursor for
        select store_id,count(distinct film_id) as films from inventory
		group by store_id;
begin
    open cur;
    loop
        fetch cur into rec;
        exit when not found;

        insert into films_in_store(store_id,number_of_films)
        values (rec.store_id,rec.films);
    end loop;

    close cur;
end;
$$;

-- 4. Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
select * from customer;
create table inactive_customers(
	customer_id int,
	customer_name text,
	last_rental_date timestamp,
	inactive_days int
);
do $$
declare
    rec record;
    cur cursor for
        select c.customer_id,c.first_name|| ' ' ||c.last_name as customer_name,max(r.rental_date) last_rental_date from customer c
		left join rental r on r.customer_id = c.customer_id
		group by c.customer_id;
begin
    open cur;
    loop
        fetch cur into rec;
        exit when not found;

		if rec.last_rental_date <= current_timestamp - interval '6 months' then
        	insert into inactive_customers(customer_id,customer_name,last_rental_date,inactive_days)
        	values (rec.customer_id,rec.customer_name,rec.last_rental_date,extract(day from current_timestamp-rec.last_rental_date));
		end if;
    end loop;

    close cur;
end;
$$;
select * from inactive_customers
order by customer_id;
--------------------------------------------------------------------------
-- Transactions
-- 1. Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.
select * from staff;
begin transaction;
	insert into customer(store_id,first_name,last_name,email,address_id,activebool,create_date,last_update,active)
	values(1,'Nithish','Udhayakumar','nithishu@gmail.com',605,true,current_timestamp,current_timestamp,1)
	returning customer_id;

	savepoint inserted_customer;

	insert into rental(rental_date,inventory_id,customer_id,return_date,staff_id,last_update)
	values(current_timestamp,1000,609,current_timestamp,1,current_timestamp)
	returning rental_id;

	savepoint inserted_rental;

	insert into payment(customer_id,staff_id,rental_id,amount,payment_date)
	values(609,1,16053,10,current_timestamp);
commit;
rollback;
select * from rental
where rental_id = 16053;

-- 2. Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
begin transaction;

	-- staff_id is 1
	update rental
	set staff_id = 2
	where rental_id = 16053;
	-- updated staff_id to 2 but not committed

	-- non-existing column
	update rental
	set rental_name = 'rental1'
	where rental_id = 16503;
	-- even commit won't work if the issue occur in transaction
	-- further queries won't work untill issue rollback

commit;
rollback;
-- staff_id is still 1
select * from rental
where rental_id = 16053;

-- 3. Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.
select * from payment
where rental_id = 16053;

begin transaction;
	update payment
	set amount = 100
	where rental_id = 16053;

	savepoint one;

	update payment
	set amount = 500
	where rental_id = 16053;

	savepoint two;
	
	update payment
	set amount = 0
	where rental_id = 16053;

	select * from payment
	where rental_id = 16053;
	rollback to savepoint two;
	
commit;
rollback;
	
-- 4. Perform a transaction that transfers inventory from one store to another (delete + insert) safely.
begin transaction;
do
$$
declare
	storeId int;
begin
	select store_id into storeId from inventory
	where inventory_id = 1;
	
	if storeId = 1 then
	    update inventory set store_id = 2 where inventory_id = 1;
	elseif storeId = 2 then
	    update inventory set store_id = 1 where inventory_id = 1;
	end if;
end
$$;
commit;

select * from inventory where inventory_id = 1;
-- 5. Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.
select * from customer
where customer_id = 609;

begin transaction;
	delete from payment where customer_id = 609;
	delete from rental where customer_id = 609;
	delete from customer where customer_id = 609;
commit;
rollback;
-----------------------------------------------------------------------------------
-- Triggers
-- 1. Create a trigger to prevent inserting payments of zero or negative amount.
create or replace function invalid_payment()
returns trigger as
$$
begin
	if new.amount <= 0 then
		raise exception 'Payment should be more than 0';
	else
		raise notice 'Payment added in the table : %',new.payment_id;
	end if;
	return new;
end;
$$ language plpgsql;

create trigger trg_invalid_payment
before insert on payment
for each row
execute function invalid_payment();

insert into payment(customer_id,staff_id,rental_id,amount,payment_date)
values(341,2,1520,-90,current_date);

-- 2. Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.

create or replace function updates_title_rental()
returns trigger as
$$
begin
	if new.title <> old.title or new.rental_rate <> old.rental_rate then
		new.last_update := current_timestamp;
	end if;
	return new;
end;
$$ language plpgsql;

create trigger trg_updates_title_rental
before update on film
for each row
execute function updates_title_rental();

update film set title = 'Academy Dinosaur' where film_id =1;
update film set rental_rate = 1.99 where film_id =1;
select * from film
where film_id =1;

-- 3. Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.
create table rental_log(
	log_id serial primary key,
	film_id int,
	rental_count int,
	log_date timestamp default current_timestamp
)

create or replace function insert_rental_log()
returns trigger as
$$
declare 
	rental_count int;
	filmId int;
begin
	select count(rental_id) into rental_count from rental
	where inventory_id = new.inventory_id and rental_date >= current_date - interval '7 days';
	
	select film_id into filmId from inventory
	where inventory_id = new.inventory_id;

	if rental_count > 3 then
		insert into rental_log(film_id,rental_count)
		values(filmId,rental_count);
	end if;

	return new;
end;
$$
language plpgsql;

create trigger trg_insert_rental_log
after insert on rental
for each row
execute function insert_rental_log();

insert into rental (rental_date, inventory_id, customer_id, staff_id)
values (current_timestamp, 5, 1, 1);

select * from rental_log;
	