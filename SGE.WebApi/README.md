# SGE – Sistema de Gestión de Expedientes
## Trabajo Práctico 2 – SGE.WebApi

---

## Cómo levantar la aplicación

```bash
# Desde la raíz de la solución:
dotnet run --project SGE.WebApi
```

La API estará disponible en: `http://localhost:5000`  
La documentación interactiva (Scalar) estará en: `http://localhost:5000/scalar/v1`

> **Nota:** La base de datos `SGE.sqlite` se crea automáticamente al primer arranque junto con el administrador semilla y los usuarios de prueba.

---

## Credenciales de los usuarios precargados

| Rol | Correo | Contraseña | Permisos |
|-----|--------|------------|----------|
| **Administrador** | `admin@sge.com` | `admin123` | Todos (es administrador) |
| **Usuario con permisos parciales** | `juan@sge.com` | `juan123` | ExpedienteAlta, ExpedienteModificacion, TramiteAlta, TramiteModificacion |
| **Usuario sin permisos (solo lectura)** | `maria@sge.com` | `maria123` | Ninguno |

---

## Orden de prueba recomendado desde Scalar

### Paso 1 – Autenticarse y obtener el token JWT

1. Ir a **Usuarios → POST /api/usuarios/login**
2. Usar las credenciales del administrador:
   ```json
   {
     "correoElectronico": "admin@sge.com",
     "contrasena": "admin123"
   }
   ```
3. Copiar el valor del campo `token` de la respuesta.
4. En Scalar, hacer clic en el candado (🔒) o en **Authorize** y pegar el token.

---

### Paso 2 – Gestión de Expedientes (con usuario administrador)

| # | Método | Ruta | Descripción |
|---|--------|------|-------------|
| 1 | `GET` | `/api/expedientes` | Listar todos los expedientes (inicialmente vacío) |
| 2 | `POST` | `/api/expedientes` | Crear un expediente → guardar el `idExpediente` devuelto |
| 3 | `GET` | `/api/expedientes/{id}` | Ver el expediente con sus trámites |
| 4 | `PUT` | `/api/expedientes/{id}/caratula` | Modificar la carátula |
| 5 | `PATCH` | `/api/expedientes/{id}/estado` | Cambiar estado manualmente |

**Body para crear expediente:**
```json
{
  "caratula": "Expediente de prueba - Nota 10"
}
```

**Body para modificar carátula:**
```json
{
  "nuevaCaratula": "Expediente modificado"
}
```

**Body para cambiar estado** (valores: `0`=Creado, `1`=ParaResolver, `2`=EnProceso, `3`=Resuelto):
```json
{
  "nuevoEstado": 2
}
```

---

### Paso 3 – Gestión de Trámites

| # | Método | Ruta | Descripción |
|---|--------|------|-------------|
| 1 | `POST` | `/api/tramites` | Agregar un trámite al expediente creado |
| 2 | `GET` | `/api/tramites/por-expediente/{expedienteId}` | Listar trámites del expediente |
| 3 | `PUT` | `/api/tramites/{id}` | Modificar el trámite |
| 4 | `DELETE` | `/api/tramites/{id}` | Eliminar el trámite |

**Body para agregar trámite** (etiquetas: `0`=Inicio, `1`=PaseAEstudio, `2`=PaseAResolucion, `3`=Resolucion):
```json
{
  "expedienteId": "<<PEGAR-ID-DEL-EXPEDIENTE>>",
  "etiqueta": 1,
  "contenido": "Este trámite pasa el expediente a estudio."
}
```

---

### Paso 4 – Operaciones exclusivas del Administrador

| # | Método | Ruta | Descripción |
|---|--------|------|-------------|
| 1 | `GET` | `/api/usuarios` | Listar todos los usuarios |
| 2 | `PUT` | `/api/usuarios/{id}/permisos` | Modificar permisos de un usuario |
| 3 | `DELETE` | `/api/usuarios/{id}` | Eliminar un usuario |

**Body para modificar permisos** (0=ExpedienteAlta, 1=ExpedienteBaja, 2=ExpedienteModificacion, 3=TramiteAlta, 4=TramiteBaja, 5=TramiteModificacion):
```json
{
  "nuevosPermisos": [0, 2, 3, 5]
}
```

---

### Paso 5 – Prueba de control de acceso (usuario sin permisos)

1. **Login** con `maria@sge.com` / `maria123` → copiar su token
2. **Autorizar** Scalar con ese token
3. Intentar `POST /api/expedientes` → debe recibir **403 Forbidden** con ProblemDetails
4. Intentar `GET /api/expedientes` → debe recibir **200 OK** (lectura libre para autenticados)

---

### Paso 6 – Prueba de control de acceso (usuario con permisos parciales)

1. **Login** con `juan@sge.com` / `juan123` → copiar su token
2. Intentar `DELETE /api/expedientes/{id}` → debe recibir **403 Forbidden** (no tiene ExpedienteBaja)
3. Intentar `POST /api/expedientes` → debe recibir **201 Created** (tiene ExpedienteAlta)

---

### Paso 7 – Prueba de registro de nuevo usuario

1. **POST** `/api/usuarios/registrar` (sin token):
   ```json
   {
     "nombre": "Nuevo Usuario",
     "correoElectronico": "nuevo@test.com",
     "contrasena": "test123"
   }
   ```
2. **Login** con las nuevas credenciales → obtendrá un token con permisos vacíos

---

## Formato de errores (ProblemDetails)

Todas las excepciones de negocio devuelven el formato estándar RFC 7807:

```json
{
  "type": null,
  "title": "Acceso denegado",
  "status": 403,
  "detail": "El usuario no tiene permisos para crear expedientes.",
  "instance": "/api/expedientes"
}
```

| Tipo de excepción | Código HTTP |
|---|---|
| `AutorizacionException` | **403** Forbidden |
| `EntidadNoEncontradaException` | **404** Not Found |
| `EntidadDuplicadaException` | **400** Bad Request |
| `DominioException` | **400** Bad Request |
| Cualquier otra | **500** Internal Server Error |

---

## Estructura de la capa de presentación (SGE.WebApi)

```
SGE.WebApi/
├── Program.cs                    ← Entry point limpio (5 fases bien separadas)
├── AplicacionExtensions.cs       ← Registro de Casos de Uso (.AddAplicacion())
├── InfraestructuraExtensions.cs  ← Registro de EF Core, repos, JWT (.AddInfraestructura())
├── DbSeedExtensions.cs           ← Inicialización de BD y datos semilla
├── SGE.WebApi.csproj
├── appsettings.json
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
└── Endpoints/
    ├── UsuariosEndpoints.cs      ← /api/usuarios (auth + CRUD admin)
    ├── ExpedientesEndpoints.cs   ← /api/expedientes
    └── TramitesEndpoints.cs      ← /api/tramites
```
