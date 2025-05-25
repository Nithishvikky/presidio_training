-- Phase 3: SQL Joins Practice

-- Write queries to:

-- 1. List students and the courses they enrolled in
-- 2. Show students who received certificates with trainer names
-- 3. Count number of students per course

select S.student_id,S.name,E.course_id,C.course_name from students S
join enrollments E on S.student_id = E.student_id
join courses C on C.course_id = E.course_id
order by S.student_id,E.course_id;

select S.student_id,S.name,Cr.course_name,t.trainer_name,C.serial_no from students S
join enrollments E on S.student_id = E.student_id
join certificates C on E.enrollment_id = C.enrollment_id
join courses Cr on E.course_id = Cr.course_id
join courseAssignment Ca on Ca.course_id = Cr.course_id
join trainers t on t.trainer_id = Ca.trainer_id;

select C.course_id,C.course_name,count(student_id) Students from courses C
join enrollments E on C.course_id = E.course_id
group by C.course_id
order by C.course_id;

