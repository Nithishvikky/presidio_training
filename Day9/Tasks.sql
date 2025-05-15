-- 1. Create a stored procedure to encrypt a given text
-- Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension.
-- Use pgp_sym_encrypt(text, key) from pgcrypto.

create extension if not exists pgcrypto;

create or replace procedure sp_encrypt_text(in p_text text,out encrypted_output bytea)
language plpgsql
as $$
begin
	encrypted_output := pgp_sym_encrypt(p_text,'test_123'); -- Hardcoded encryption_key = 'test_123'
end;
$$;

do $$
declare
	res bytea;
begin
	call sp_encrypt_text('nithishu@gmail.com',res);
	raise notice 'Encrypted(base64 text) : %',encode(res,'base64');
end;
$$;
 
-- 2. Create a stored procedure to compare two encrypted texts
-- Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text.

create or replace procedure sp_compare_encrypted(in encrypted_text1 bytea,in encrypted_text2 bytea)
language plpgsql as
$$
begin
	-- -- Hardcoded encryption_key = 'test_123'
	if pgp_sym_decrypt(encrypted_text1,'test_123') = pgp_sym_decrypt(encrypted_text2,'test_123') then 
		raise notice 'Both are same plain text';
	else
		raise notice 'Both are not same plain text';
	end if;
end;
$$;

do $$
declare
	res1 bytea;
	res2 bytea;
begin
	call sp_encrypt_text('nithishu@gmail.com',res1);
	call sp_encrypt_text('nithishu@gmail.com',res2);
	call sp_compare_encrypted(res1,res2);
end;
$$;
 
-- 3. Create a stored procedure to partially mask a given text
-- Task: Write a procedure sp_mask_text that:
-- Shows only the first 2 and last 2 characters of the input string
-- Masks the rest with *
-- E.g., input: 'john.doe@example.com' → output: 'jo***************om'

create or replace procedure sp_mask_text(in original_text text,out masked_text text)
language plpgsql
as $$
declare
	vs text; 
	ve text; 
	mm text; 
begin
	if length(original_text) <= 4 then
		masked_text := original_text;
	else 
		masked_text := left(original_text,2)||repeat('*',length(original_text)-4)||right(original_text,2);
	end if;
end;
$$;

do $$
declare
	masked_email text;
begin
	call sp_mask_text('nithish',masked_email);
	raise notice '%',masked_email;
end;
$$;
	
-- 4. Create a procedure to insert into customer with encrypted email and masked name
-- Task:
 
-- Call sp_encrypt_text for email
-- Call sp_mask_text for first_name
-- Insert masked and encrypted values into the customer table
-- Use any valid address_id and store_id to satisfy FK constraints.

create table customer_dummy(
	customer_id serial primary key,
	first_name text,
	last_name text,
	email text
);

create or replace procedure sp_insert_customer_dummy(in user_data json)
language plpgsql
as $$
declare
	firstName text;
	encrypt_email bytea;
begin
	call sp_mask_text(user_data ->> 'first_name',firstName);
	call sp_encrypt_text(user_data ->> 'email',encrypt_email);
	
	insert into customer_dummy(first_name,last_name,email)
	values(firstName,user_data ->>'last_name',encode(encrypt_email,'base64'));
end;
$$;

call sp_insert_customer_dummy('{"first_name":"ajay","last_name":"kulandaivel","email":"ajayk@gmail.com"}');

select * from customer_dummy;

-- 5. Create a procedure to fetch and display masked first_name and decrypted email for all customers
-- Task:
-- Write sp_read_customer_masked() that:
-- Loops through all rows
-- Decrypts email
-- Displays customer_id, masked first name, and decrypted email

create or replace procedure sp_show_customers()
language plpgsql as
$$
declare
	cur cursor for
		select * from customer_dummy;
	rec record;
	decryped_email text;
	
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		-- Hardcoded encryption_key = 'test_123'
		decryped_email := pgp_sym_decrypt(decode(rec.email,'base64'),'test_123');
		raise notice 'First_Name : % , Email : %', rec.first_name,decryped_email;
	end loop;
	close cur;
end;
$$; 
call sp_show_customers();