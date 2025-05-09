-- Write a cursor that loops through all films and prints titles longer than 120 minutes.
do $$
declare
	rec record;
	cur cursor for
		select title,length from film;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		if rec.length > 120 then
			raise notice 'Title Name : % Length : %',rec.title,rec.length;
		end if;
	end loop;
	close cur;
end $$;

-- Create a cursor that iterates through all customers and counts how many rentals each made.
do $$
declare
	rec record;
	Rental_count int;
	cur cursor for
		select customer_id,concat(first_name,' ',last_name) CustomerName from customer
		order by customer_id;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;
		
		select count(*) into Rental_count from rental
		where rental.customer_id = rec.customer_id;

		raise notice 'Customer Id : % ,Customer Name : % , RentalCount: % ',rec.customer_id,rec.CustomerName,Rental_count;
		
	end loop;
	close cur;
end $$;

-- Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
do
$$
declare
    rec RECORD;
    rental_count INT;
begin
    for rec in
        select film_id, title, rental_rate from film
    loop
        select count(*) into rental_count
        from inventory i
        join rental r on r.inventory_id = i.inventory_id
        where i.film_id = rec.film_id;
 
        if rental_count < 5 then
            update film set rental_rate = rental_rate + 1
            where film_id = rec.film_id;
 
            raise notice 'Updated: %, Rentals: %, New Rate: %', 
                rec.title, rental_count, rec.rental_rate + 1;
        end if;
    end loop;
end;
$$ 
language plpgsql;

-- Create a function using a cursor that collects titles of all films from a particular category.
create or replace function getAllflims(categoryName TEXT)
returns void as $$
declare
    cur cursor for select f.title from film f
        join film_category fc on f.film_id = fc.film_id
       	join category c on fc.category_id = c.category_id
		where c.name ILIKE categoryName;
    film_title TEXT;
begin
    open cur;
 
    loop
        fetch cur INTO film_title;
        exit when not found;
        raise notice 'Film Title: %', film_title;
    end loop;
 
    close cur;
end;
$$ 
language plpgsql;
select  getAllflims('Action');

-- Loop through all stores and count how many distinct films are available in each store using a cursor.
do 
$$
declare
    rec RECORD;
    film_count INT;
begin
    for rec in select store_id from store 
	loop
        select count(distinct film_id) into film_count from inventory
        where store_id = rec.store_id;
		
        raise notice 'Store ID: %, Distinct Films: %', rec.store_id, film_count;
    end loop;
end;
$$;
--------------------------------------
-- triggers
-- Write a trigger that logs whenever a new customer is inserted.
create or replace function customer_inserted()
returns trigger as
$$
begin
	raise notice 'New customer inserted (customer_id) : %',new.customer_id;
	return new;
end;
$$ language plpgsql;

create trigger trg_customer_inserted
after insert on customer
for each row
execute function customer_inserted();

insert into customer(store_id,first_name,last_name,email,address_id)
values(1,'Nithish','U','nithi@sakilcustomer.org',1);

-- Create a trigger that prevents inserting a payment of amount 0.
select * from payment
where payment_id = 32106;

create or replace function invalid_payment()
returns trigger as
$$
begin
	if new.amount = 0 then
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
values(341,2,1520,150,current_date);

-- Set up a trigger to automatically set last_update on the film table before update.
create or replace function updateLast_update()
returns trigger as
$$
begin
    NEW.last_update := CURRENT_TIMESTAMP;
    return new;
end;
$$ 
language plpgsql;
 
create trigger trg_updateLast_update
before update on film
for each row
execute function updateLast_update();

-- Create a trigger to log changes in the inventory table (insert/delete).
create table inventory_log (
    log_id serial primary key,
    action_type varchar(10), 
    inventory_id int,
    log_time timestamp default current_timestamp
);
 
create or replace function log_inventory_changes()
returns trigger as 
$$
begin
    if (TG_OP = 'INSERT') then
        insert into inventory_log(action_type, inventory_id)
        values ('INSERT', NEW.inventory_id);
    elseif (TG_OP = 'DELETE') then
        insert into inventory_log(action_type, inventory_id)
        values ('DELETE', OLD.inventory_id);
    end if;
    return null;
end;
$$ 
language plpgsql;
 
create trigger trg_log_inventory_changes
after insert or delete on inventory
for each row
execute function log_inventory_changes();

-- Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
create or replace function check_outstanding_balance()
returns trigger as 
$$
declare
    outstanding_balance DECIMAL;
begin
    select sum(amount) into outstanding_balance from payment
    where customer_id = NEW.customer_id and payment_date > current_date - INTERVAL '30 days';
 
    if outstanding_balance > 50 then
        raise exception 'Customer owes more than $50';
    end if;
 
    return new;
end;
$$ 
language plpgsql;
 
create trigger trigger_check_outstanding_balance
before insert on rental
for each row
execute function check_outstanding_balance();

-- Transaction

-- Write a transaction that inserts a customer and an initial rental in one atomic operation.
begin transaction;
do 
$$ 
declare
    cust_id INT;
begin
    insert into customer (first_name, last_name, email, address_id, active, create_date, store_id)
    VALUES ('nithish', 'U', 'nithish@example.com', 1, 1, current_date(), 1)
    returning customer_id into cust_id;
 
    insert into rental (rental_date, inventory_id, customer_id, return_date, staff_id)
    values (current_date(), 1, cust_id, NULL, 1);
end
$$;
commit;
 
rollback
 
-- Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
begin transaction;
 
update film set rental_duration = 5
where title = 'ACADEMY DINOSAUR';
 
insert into inventory (film_id, store_id, last_update)
VALUES (9990, 1, current_date());
 
commit;
rollback
 
-- Create a transaction that transfers an inventory item from one store to another.
begin transaction;
do $$
declare
    Nstore_id INTEGER;
begin
    select store_id into Nstore_id from inventory
    where inventory_id = 98;
 
    if Nstore_id = 1 then
        update inventory set store_id = 2 where inventory_id = 98;
    elseif Nstore_id = 2 then
        update inventory set store_id = 1 where inventory_id = 98;
    end if;
end $$;
commit;
 
-- Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.
begin transaction;

update payment set amount = amount + 1 
where payment_id = 1;

savepoint after_first_update;

update payment set amount = amount + 2 
where payment_id = 1;

rollback to savepoint after_first_update;

update payment set amount = amount + 3 
where payment_id = 1;

commit;
 
-- Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.
begin transaction;

delete from payment where customer_id = 110;
delete from rental where customer_id = 110;
delete from customer where customer_id = 110;

commit;