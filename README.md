# 🅿️ EstacionamentoCrud — Sistema Completo de Gestão de Estacionamento
![ASP.NET](https://img.shields.io/badge/ASP.NET%20Web%20Forms-%230078D7.svg?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-%23CC2927.svg?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap%205-%237952B3.svg?style=for-the-badge&logo=bootstrap&logoColor=white)
![CSharp](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)

## 🧭 Sobre o Projeto
O **EstacionamentoCrud** é um sistema moderno e intuitivo para o **controle completo de veículos** em estacionamentos.  
Desenvolvido em **ASP.NET Web Forms (C#)** com **SQL Server**, o sistema realiza **cadastro, cálculo de permanência, baixa de veículos e controle financeiro** de forma totalmente automatizada.  
💡 Ideal para **gestores de estacionamento, lava-rápidos e condomínios**, com foco em **praticidade, automação e eficiência.**

## ⚙️ Funcionalidades
✅ Cadastro de veículos (Placa, Modelo, Cor)  
✅ Registro automático da **Data e Hora de Entrada**  
✅ Cálculo automático de **tempo de permanência e valor total**  
✅ Função **“Dar Baixa”** com registro da **Data/Hora de Saída**  
✅ Atualização automática do **status (Estacionado / Finalizado)**  
✅ **Bloqueio de edição** para veículos finalizados (somente exclusão permitida)  
✅ Interface moderna e responsiva com **Bootstrap 5**  
✅ Telas de **Login** e **Home** com autenticação simples  

## 💰 Regras de Cobrança

⏱ Até 15 minutos = Gratuito

🕐 Primeiras 2 horas = R$ 18,00 

🕐 Cada hora adicional = R$ 5,00


O cálculo é feito automaticamente no momento da baixa do veículo.

## 🧱 Estrutura do Banco de Dados
**Banco:** EstacionamentoDB  
**Tabela:** Veiculos  

```sql
CREATE TABLE Veiculos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Placa NVARCHAR(10) NOT NULL,
    Modelo NVARCHAR(50),
    Cor NVARCHAR(30),
    DataEntrada DATETIME NOT NULL,
    HoraEntrada NVARCHAR(10) NOT NULL,
    DataSaida DATETIME NULL,
    ValorPago DECIMAL(10,2) NULL,
    Status NVARCHAR(20) NOT NULL
);
````
## 🧩 Tecnologias Utilizadas

| 💡 Tecnologia | 📘 Descrição |
|---------------|--------------|
| ![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) | 🧠 **Lógica de negócio e backend** com .NET Framework 4.8 |
| ![ASP.NET](https://img.shields.io/badge/ASP.NET%20Web%20Forms-%230078D7.svg?style=for-the-badge&logo=dotnet&logoColor=white) | 🌐 **Estrutura de páginas e eventos** |
| ![SQL Server](https://img.shields.io/badge/SQL%20Server-%23CC2927.svg?style=for-the-badge&logo=microsoftsqlserver&logoColor=white) | 🗄️ **Banco de dados relacional** |
| ![Bootstrap 5](https://img.shields.io/badge/Bootstrap%205-%237952B3.svg?style=for-the-badge&logo=bootstrap&logoColor=white) | 🎨 **Layout responsivo e estilização moderna** |
| ![HTML5](https://img.shields.io/badge/HTML5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white) ![CSS3](https://img.shields.io/badge/CSS3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white) ![JavaScript](https://img.shields.io/badge/JavaScript-%23F7DF1E.svg?style=for-the-badge&logo=javascript&logoColor=black) | 🧩 **Camada visual e interatividade** |

## 👨‍💻 Autor

| 🧾 Informação | 💬 Detalhes |
|---------------|-------------|
| 👤 **Nome** | Douglas Henrique |
| 📍 **Localização** | Campinas - SP |
| 🎓 **Formação** | Estudante de **Análise e Desenvolvimento de Sistemas** |
| 💼 **Atuação** | Desenvolvedor **Web / Backend** |
| 🔗 **LinkedIn** | [![LinkedIn](https://img.shields.io/badge/LinkedIn-blue?logo=linkedin&logoColor=white&style=for-the-badge)](https://www.linkedin.com/in/douglashenrique76/) |
| 💻 **GitHub** | [![GitHub](https://img.shields.io/badge/GitHub-black?logo=github&logoColor=white&style=for-the-badge)](https://github.com/henriquedgs76) |

---

🚀 **Projeto desenvolvido com dedicação, foco em boas práticas e experiência real de uso.**  
> 💡 *“Cada linha de código aproxima a tecnologia do mundo real.”*
