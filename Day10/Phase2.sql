-- 1. **students**
--    * `student_id (PK)`, `name`, `email`, `phone`
-- 2. **courses**
--    * `course_id (PK)`, `course_name`, `category`, `duration_days`
-- 3. **trainers**
--    * `trainer_id (PK)`, `trainer_name`, `expertise`
-- 4. **enrollments**
--    * `enrollment_id (PK)`, `student_id (FK)`, `course_id (FK)`, `enroll_date`
-- 5. **certificates**
--    * `certificate_id (PK)`, `enrollment_id (FK)`, `issue_date`, `serial_no`
-- 6. **course\_trainers** (Many-to-Many if needed)
--    * `course_id`, `trainer_id`

-- Student Table
create table students(
student_id serial primary key,
name text,
email text,
phone text
);
-- Courses table
create table courses(
course_id serial primary key,
course_name text,
category text,
duration_days int
);
-- Trainers table
create table trainers(
trainer_id serial primary key,
trainer_name text,
expertise text
);
-- Enrollment table
create table enrollments(
enrollment_id serial primary key,
student_id int not null,
course_id int not null,
enroll_date timestamp default current_timestamp,
foreign key (student_id) references students(student_id),
foreign key (course_id) references courses(course_id)
);
-- Certificate Table
create table certificates(
certificate_id serial primary key,
enrollment_id int not null,
issue_date timestamp default current_timestamp,
serial_no text unique not null,
foreign key (enrollment_id) references enrollments(enrollment_id)
);
-- CourseAssignment Table
create table courseAssignment(
course_id int not null references courses(course_id),
trainer_id int not null references trainers(trainer_id),
primary key(trainer_id,course_id)
);
--------------------------
--Insert
select * from students;
INSERT INTO students (name, email, phone) VALUES
('Aarav Kumar', 'aarav.kumar@example.com', '9876543210'),
('Diya Sharma', 'diya.sharma@example.com', '9876543211'),
('Rohan Mehta', 'rohan.mehta@example.com', '9876543212'),
('Anaya Iyer', 'anaya.iyer@example.com', '9876543213'),
('Vivaan Reddy', 'vivaan.reddy@example.com', '9876543214'),
('Meera Nair', 'meera.nair@example.com', '9876543215'),
('Arjun Das', 'arjun.das@example.com', '9876543216'),
('Ishita Rao', 'ishita.rao@example.com', '9876543217'),
('Krishna Patel', 'krishna.patel@example.com', '9876543218'),
('Saanvi Joshi', 'saanvi.joshi@example.com', '9876543219');

select * from courses;
INSERT INTO courses (course_name, category, duration_days) VALUES
('Introduction to Python', 'Programming', 30),
('Data Structures and Algorithms', 'Computer Science', 45),
('Digital Marketing Basics', 'Marketing', 25),
('Graphic Design Fundamentals', 'Design', 40),
('Business Communication', 'Business', 20),
('Web Development Bootcamp', 'Programming', 60),
('Financial Accounting', 'Finance', 35),
('Machine Learning Essentials', 'Data Science', 50),
('Creative Writing Workshop', 'Arts', 15),
('Project Management Principles', 'Management', 30);

select * from trainers;
INSERT INTO trainers (trainer_name, expertise) VALUES
('Rajesh Verma', 'Python Programming'),
('Sneha Kapoor', 'Digital Marketing'),
('Amit Reddy', 'Data Science'),
('Priya Nair', 'Graphic Design'),
('Vikram Joshi', 'Business Communication'),
('Neha Sharma', 'Web Development'),
('Arun Mehta', 'Financial Accounting'),
('Divya Rao', 'Machine Learning'),
('Karan Singh', 'Creative Writing'),
('Meena Iyer', 'Project Management');

select * from courseAssignment;
INSERT INTO courseAssignment (trainer_id, course_id) VALUES
(1, 1), 
(1, 2),  
(2, 3), 
(3, 8),  
(3, 2),  
(4, 4),  
(5, 5),  
(6, 6), 
(7, 7),
(10, 10); 

select * from enrollments;
INSERT INTO enrollments (student_id, course_id) VALUES
(1, 1),
(2, 3),
(3, 2),
(4, 5),
(5, 4),
(6, 6),
(7, 7),
(8, 8),
(9, 9),
(10, 10);

select * from  certificates;
INSERT INTO certificates (enrollment_id, serial_no) VALUES 
(8,'CERT250008');
(1, 'CERT250001'),
(2, 'CERT250002'),
(3, 'CERT250003'),
(4, 'CERT250004'),
(5, 'CERT250005'),
(6, 'CERT250006'),
(7, 'CERT250007'),
(9, 'CERT250009'),
(10, 'CERT250010');

--------------------
-- indices
create index idx_email_id on students(email,student_id);
create index idx_courseId on courses(course_id);
create index idx_enrollments on enrollments(course_id,student_id);
create index idx_course_trainer_Id on courseAssignment(course_id);