
create table if not exists Users(
  UserId serial primary key,
  UserName varchar(20) not null,
  UserEmail varchar(50) not null unique,
  UserPasswordHash varchar(256) not null,
  UserPasswordSalt varchar(256) not null
);

create table if not exists Notes(
    NoteId serial primary key,
    UserId int references Users(UserId),
    NoteText text,
    NoteCreatedTime timestamp without time zone
);