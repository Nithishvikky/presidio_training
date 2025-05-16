INSERT INTO students (name, email, phone) VALUES
('Nithish U', 'nithish.Udhaya@example.com', '8825823783');

INSERT INTO enrollments (student_id, course_id) VALUES
(11, 3);

-- It won't allow
select * from students;