
-- RACEDAY DATABASE --

CREATE DATABASE RaceDayDB;

-- OPENING OR USE THE DATABASE --
USE RaceDayDB;


-- CREATE TABLES --
-- 1. users table (shared authentication for both roles) --
create table users(
	user_id int primary key identity(1,1),
	email varchar(100) not null unique,
	password_hash varchar(255) not null,
	role varchar(20) not null check (role in ('Organiser','Participant')),
	created_at datetime not null default getdate()
);

-- 2. organisers table --
create table organisers(
	org_id int primary key identity(1,1),
	full_name varchar(100) not null,
	phone_num varchar(20) not null,
	user_id int not null unique foreign key references users(user_id)
);

-- 3. participants table --
create table participants(
	participant_id int primary key identity(1,1),
	full_name varchar(100) not null,
	id_num varchar(13) not null unique,
	address varchar(255) null,
	user_id int not null unique foreign key references users(user_id)
);
	
-- 4. event table --
create table events(
	event_id int primary key identity(1,1),
	event_name varchar(150) not null,
	event_description varchar(max),
	date date not null,
	location varchar(150) not null,
	distance_km decimal(6,2) not null,
	event_type varchar(10) not null check (event_type in ('Run','Walk','Cycle')),
	org_id int not null foreign key references organisers (org_id)
);

-- 5. categories table --
create table categories(
	category_id int primary key identity(1,1),
	category_name varchar(150) not null,
	description  varchar(max),
	category_type varchar(10) not null check (category_type in ('Age','Distance')),
	age_group varchar(20) null,
	distance_km decimal(5,2) null,
	event_id int not null foreign key references events (event_id),
	check (
		(category_type = 'Age' and age_group is not null and distance_km is null)
		or
		(category_type = 'Distance' and distance_km is not null and age_group is null)
	)
);

-- 6. enrolments table --
create table enrolments(
	enrol_id int primary key identity(1,1),
	enrol_date date not null default getdate(),
	category_id int not null foreign key references categories(category_id),
	participant_id int not null foreign key references participants (participant_id),
	unique(participant_id, category_id)
);

-- 7. result table --
create table results(
	result_id int primary key identity(1,1),
	duration time,
	average_speed decimal(5,2),
	position int,
	enrol_id int not null unique foreign key references enrolments(enrol_id)
);


-- INSERT SEED DATA --
-- 1. Users table (login credentials + role) --
insert into users (email, password_hash, role)
values
('sipho@gmail.co.za', 'Sipho@12345', 'Organiser'),
('lindiwe@gmail.co.za', 'Lindiwe@12345', 'Organiser'),
('johan@gmail.com', 'Johan@12345', 'Participant'),
('naledi@gmail.com', 'Naledi@12345', 'Participant');

-- 2. Organiser table --
insert into organisers (user_id, full_name, phone_num)
values
(1, 'Sipho Ndlovu', '0821234567'),
(2, 'Lindiwe Zulu', '0839876543');

-- 3. Participant table --
insert into participants (user_id, full_name, id_num, address)
values
(3, 'Johan Botha', '9001015800083', '12 Oak St, Pretoria'),
(4, 'Naledi Khumalo', '9506120123456', '45 Vilakazi St, Soweto');

-- 4. Event table --
insert into events (event_name, location, event_description, date, distance_km, event_type, org_id)
values
('Comrades Marathon', 'Pietermaritzburg to Durban', 'Iconic ultra-marathon between PMB and Durban', '2027-06-13', 89.00, 'Run', 1),
('Two Oceans Marathon', 'Cape Town', 'Scenic ultra around the Cape Peninsula', '2027-04-04', 56.00, 'Run', 2),
('Soweto Marathon', 'Soweto', 'Community road race through Soweto', '2026-11-08', 42.20, 'Run', 1);

-- 5. Category table (each category is either age-based or distance-based) --
insert into categories (category_name, description, category_type, age_group, distance_km, event_id)
values
('Senior Open', 'Standard entry, no age restriction', 'Age', 'Open', null, 1),
('Veteran', 'For runners 40+', 'Age', '40+', null, 1),
('21km', 'Half-marathon distance category', 'Distance', null, 21.00, 2),
('56km', 'Full ultra distance category', 'Distance', null, 56.00, 2),
('10km', '10km distance category', 'Distance', null, 10.00, 3),
('Under 18', 'Junior age category', 'Age', 'Under 18', null, 3);

-- 6. Enrolment table (links participant to category; event is reached via category.event_id) --
insert into enrolments (enrol_date, category_id, participant_id)
values
('2027-01-10', 1, 1),
('2027-01-15', 3, 2),
('2026-09-01', 5, 1);

-- 7. Result table --
insert into results (duration, average_speed, position, enrol_id)
values
('09:15:32', 9.70, 1523, 1),
('01:45:10', 12.00, 87, 2);