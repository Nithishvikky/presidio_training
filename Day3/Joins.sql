
use pubs;

-- Print Auhtor and their books

	select au_id,title from titleauthor
	left join titles on titles.title_id = titleauthor.title_id;

-- Print Author's name with their book name

	select concat(au_fname,' ',au_lname) Author_name,title as Bookname from authors a
	join titleauthor ta on ta.au_id = a.au_id
	join titles t on t.title_id = ta.title_id;

-- Print the publisher's name, book name and the order date of the  books

	select pub_name publisher_name,title book_name, ord_date from publishers p
	join titles t on t.pub_id = p.pub_id
	join sales s on s.title_id = t.title_id;

-- Print the publisher name and the first book sale date for all the publishers

	select pub_name publisher_name,min(ord_date) from publishers p
	left join titles t on t.pub_id = p.pub_id
	left join sales s on s.title_id = t.title_id
	group by p.pub_name;