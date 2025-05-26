-- Phase 4: Functions & Stored Procedures

-- Function:
-- Create `get_certified_students(course_id INT)`
-- → Returns a list of students who completed the given course and received certificates.

-- Stored Procedure:
-- Create `sp_enroll_student(p_student_id, p_course_id)`
-- → Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).

create or replace function get_certified_students(courseId int)
returns table (id int,name text,courseName text,serialNo text)
as $$
begin
	return query
	select S.student_id as id,S.name as name,Cr.course_name as courseName,C.serial_no as serialNo from students S
	join enrollments E on S.student_id = E.student_id
	join certificates C on E.enrollment_id = C.enrollment_id
	join courses Cr on E.course_id = Cr.course_id
	where Cr.course_id = courseId;
end;
$$ language plpgsql;

select * from get_certified_students(1);

create or replace procedure sp_enroll_student(in p_student_id int,in p_course_id int,in p_completed boolean)
language plpgsql
as $$
declare
	enrollmentId int;
	serialNo text;
begin
	insert into enrollments(student_id,course_id)
	values(p_student_id,p_course_id)
	returning enrollment_id into enrollmentId;

	if p_completed then
		serialNo := 'CERT'||To_char(now(),'YY')||lpad(enrollmentId::text,4,'0');
		insert into certificates(enrollment_id,serial_no)
		values (enrollmentId,serialNo);
	end if;
end;
$$;

call sp_enroll_student(1,2,false);