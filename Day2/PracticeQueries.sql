use pubs;

select title from titles;

select title from titles
where pub_id = 1389;

select title from titles
where price between 10 and 15;

select title from titles
where price is null;

select title from titles
where title like 'The%';

select title from titles
where title not like '%v%';

select title from titles
order by royalty;

select * from titles
order by pub_id desc,type,price desc;

select type,avg(price) as average from titles
group by type;

select distinct type from titles;

select title,price from titles
order by price desc;

select title from titles
where type = 'business' and price<20 and advance>7000;

select * from authors
where state='CA';

select state,count(au_id) as authors_count from authors
group by state;

select pub_id,count(*) as no_of_books from titles
where price>=15 and price<=25 and title like '%It%'
group by pub_id
having count(*)>2
order by no_of_books;

