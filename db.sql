create table arquivos(
	IdArquivo int identity(1,1) not null primary key,
	NomeArquivo varchar(650) not null,
	ContentType varchar(50) not null,
	IdDepartamento int not null foreign key references Departamento(IdDepartamento),
	IdTipoDoc int not null foreign key references TipoDocumento(IdTipoDoc),
	IdUsuarioRemetente int not null foreign key references usuarios(IdUsuario),
	IdUsuarioDestinatario int not null foreign key references usuarios(IdUsuario),
	DataUpload datetime not null,
	Descricao varchar(1500) not null,
	DataUltDownload datetime null,
	Contdownloads int not null,
	arquivo varbinary(MAX)
);
CREATE TABLE usuarios(
	IdUsuario int IDENTITY(1,1) NOT NULL primary key,
	NomeUsuario varchar(25) NOT NULL,
	IdTipoUsu  int not null foreign key references TipoUsuario(IdTipoUsu),
	Email varchar(75) NOT NULL,
	Senha varchar(max) NOT NULL,
);

create table Departamento(
	IdDepartamento int identity (1,1) primary key,
	Nome Departamento varchar (60)
);	

create table TipoDocumento(
	IdTipoDoc int identity (1,1) primary key,
	TipoDoc varchar (60)
);

create table TipoUsuario(
	IdTipoUsu int identity (1,1) primary key,
	TipoUsu varchar (60)
);


insert into TipoUsuario (TipoUsu) values ('cliente');
insert into TipoUsuario (TipoUsu) values ('usuario');

/*Usuário teste, IdTipoUsu 1 = cliente; IdTipoUsu 2 = usuario OBS: Senha: 123456 (Base64) */
insert into usuarios (NomeUsuario,IdTipoUsu,Email,Senha) values ('Nome de usuário',1,'email@email.com','MTIzNDU2');


