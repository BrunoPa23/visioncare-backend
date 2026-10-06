# VisionCare Backend

[![CI](https://github.com/BrunoPa23/visioncare-backend/actions/workflows/ci.yml/badge.svg)](https://github.com/BrunoPa23/visioncare-backend/actions/workflows/ci.yml)

> **Muestra publica de portafolio.** Version publica y saneada del backend de VisionCare, proyecto final del curso Seminario de Investigacion Academica (Ingenieria de Software, UPC, 2025), desarrollado en equipo por Bruno Palomino y William Riega. No incluye configuracion sensible, artefactos de compilacion ni el pipeline de despliegue original. La app movil esta en [visioncare-mobile](https://github.com/BrunoPa23/visioncare-mobile).

VisionCare ayuda a las personas a gestionar sus medicamentos: reconoce el texto de la etiqueta de un medicamento a partir de una foto, lo interpreta con un asistente de OpenAI y permite programar recordatorios de toma desde la app movil.

## Funcionalidades

- Registro, inicio de sesion, refresh token y cierre de sesion con JWT (token por header o cookie)
- Contrasenas con hash BCrypt
- Reconocimiento de texto en fotos de medicamentos con Azure AI Vision (OCR)
- Interpretacion del texto reconocido con un asistente de OpenAI
- Gestion de medicamentos por usuario y de sus horarios de toma

## Arquitectura

Monolito modular organizado por bounded contexts siguiendo Domain Driven Design:

```
VisionCareCore/
├── User/         Autenticacion, usuarios, JWT y BCrypt
├── HealthCare/   Medicamentos y horarios de toma
├── Vision/       Reconocimiento de imagenes con Azure AI Vision
├── OpenAI/       Integracion con el asistente de OpenAI
└── Shared/       AppDbContext, repositorio base, Unit of Work y convenciones de EF Core
```

| Capa | Responsabilidad |
|---|---|
| Domain | Entidades, agregados e interfaces de repositorios y servicios |
| Application | Command services y query services (lecturas y escrituras separadas) |
| Infrastructure | EF Core sobre SQL Server, clientes de APIs externas, hashing y tokens |
| Interfaces | Controllers REST, recursos y assemblers; ACL entre contextos |

## Endpoints

| Metodo | Ruta | Descripcion |
|---|---|---|
| POST | /vc/v1/authentication/sign-up | Registro de usuario |
| POST | /vc/v1/authentication/sign-in | Inicio de sesion, devuelve JWT |
| POST | /vc/v1/authentication/refresh-token | Renueva el token |
| POST | /vc/v1/authentication/sign-out | Cierra la sesion |
| GET | /vc/v1/authuser/me | Usuario autenticado |
| GET | /vc/v1/authuser | Lista de usuarios |
| GET | /vc/v1/authuser/{authUserId} | Usuario por id |
| POST | /vc/v1/vision/recognize-image | OCR de la foto de un medicamento |
| POST | /vc/v1/vision/scan | Escaneo de medicamento (OCR e interpretacion) |
| POST | /vc/v1/gpt/test | Procesa texto con el asistente de OpenAI |
| GET | /vc/v1/gpt/test-connection | Verifica la conexion con OpenAI |
| POST | /api/medicine | Registra un medicamento |
| GET | /api/medicine/user/{userId} | Medicamentos de un usuario |
| DELETE | /api/medicine/{id} | Elimina un medicamento |
| POST | /api/medicine-time | Programa un horario de toma |
| POST | /api/medicine-time/update-all | Actualiza varios horarios |
| GET | /api/medicine-time/by-medicine/{medicineId} | Horarios de un medicamento |
| GET | /api/medicine-time/{id} | Horario por id |
| DELETE | /api/medicine-time/{id} | Elimina un horario |

La documentacion interactiva esta disponible en Swagger al ejecutar la API.

## Stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 con SQL Server
- JWT Bearer y BCrypt.Net
- Azure AI Vision (Image Analysis) y OpenAI
- Swagger (Swashbuckle)

## Como ejecutarlo

Requisitos: .NET 8 SDK (o superior) y una instancia de SQL Server.

1. Configura la base de datos y el secreto JWT con user-secrets:

```bash
cd VisionCareCore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena de conexion a SQL Server>"
dotnet user-secrets set "TokenSettings:Secret" "<clave de al menos 32 caracteres>"
```

2. Define las claves de los servicios externos como variables de entorno:

| Variable | Uso |
|---|---|
| APPSETTING_Vision_Endpoint | Endpoint de Azure AI Vision |
| APPSETTING_Vision_Key | Clave de Azure AI Vision |
| APPSETTING_OpenAI_Key | Clave de la API de OpenAI |
| APPSETTING_Assistant_Key | Id del asistente de OpenAI |

3. Ajusta los origenes permitidos de CORS en `appsettings.json` (seccion `Cors:OrigenesPermitidos`).

4. Ejecuta la API y abre Swagger en la URL que muestra la consola:

```bash
dotnet run --project VisionCareCore
```

## Equipo

- Bruno Palomino ([@BrunoPa23](https://github.com/BrunoPa23))
- William Riega
