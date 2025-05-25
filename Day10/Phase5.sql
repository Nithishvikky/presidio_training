-- Phase 5: Cursor
-- Use a cursor to:
-- * Loop through all students in a course
-- * Print name and email of those who do not yet have certificates

select S.student_id,S.name name,S.email email,C.course_name,E.enrollment_id enrollmentId from students S
join enrollments E on E.student_id = S.student_id
join courses C on C.course_id = E.course_id
where E.course_id = 1;

create or replace procedure get_noncertitfied_students(in courseId int)
language plpgsql
as $$
declare 
	rec record;
	cur cursor for
		select S.student_id,S.name name,S.email email,C.course_name,E.enrollment_id enrollmentId from students S
		join enrollments E on E.student_id = S.student_id
		join courses C on C.course_id = E.course_id
		where E.course_id = courseId;
		
	CertificateId int;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		select C.certificate_id into CertificateId from certificates C
		where C.enrollment_id = rec.enrollmentId;

		if CertificateId is null then
			raise notice 'Name : % , Email : % ',rec.name,rec.email;
		end if;
	end loop;
	close cur;
end;
$$;

call get_noncertitfied_students(1);