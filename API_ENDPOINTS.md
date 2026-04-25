# MCPSitRep - API Endpoints Documentation

## Endpoints añadidos al proyecto

Se han agregado varios endpoints HTTP para exponer la funcionalidad del servicio de análisis de IA. Todos los endpoints están disponibles en el puerto **5010**.

### 1. Health Check
**Endpoint:** `GET /api/health`

Verifica que el servicio está funcionando correctamente.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "service": "MCPSitRep",
  "version": "1.0"
}
```

**curl:**
```bash
curl http://localhost:5010/api/health
```

---

### 2. Weather Analysis
**Endpoint:** `POST /api/analysis/weather`

Analiza datos meteorológicos usando Gemini AI.

**Request Body:**
```json
{
  "rawData": "{\"temperature\": 22.5, \"humidity\": 65, \"windSpeed\": 5, \"weatherType\": \"Sunny\", \"precipitation\": 0}",
  "parsedData": {
    "temperature": 22.5,
    "humidity": 65,
    "relativeHumidity": 65,
    "windSpeed": 5,
    "weatherType": "Sunny",
    "precipitation": 0
  },
  "includeSafetyRecommendations": true,
  "includeTrendAnalysis": true
}
```

**Response:**
```json
{
  "success": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "dataType": "WEATHER_ANALYSIS",
  "analysis": {
    "temperatureAnalysis": {
      "currentTemperature": 22.5,
      "comfortLevel": "optimal",
      "comfortScore": 10
    },
    "humidityAnalysis": {
      "relativeHumidity": 65,
      "humidityStatus": "ideal",
      "humidityScore": 10
    },
    "weatherConditions": {
      "windSpeed": 5,
      "weatherType": "Sunny",
      "overallWeatherStatus": "excellent",
      "weatherScore": 10
    }
  }
}
```

**curl:**
```bash
curl -X POST http://localhost:5010/api/analysis/weather \
  -H "Content-Type: application/json" \
  -d '{
    "rawData": "{\"temperature\": 22.5, \"humidity\": 65}",
    "includeSafetyRecommendations": true,
    "includeTrendAnalysis": true
  }'
```

---

### 3. Occupancy Analysis
**Endpoint:** `POST /api/analysis/occupancy`

Analiza datos de ocupación de espacios usando Gemini AI.

**Request Body:**
```json
{
  "rawData": "{\"currentOccupancy\": 45, \"maxCapacity\": 100, \"occupancyPercentage\": 45}",
  "parsedData": {
    "currentOccupancy": 45,
    "maxCapacity": 100,
    "occupancyPercentage": 45,
    "occupancyLevel": "Moderado"
  },
  "includeAlerts": true
}
```

**Response:**
```json
{
  "success": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "dataType": "OCCUPANCY_ANALYSIS",
  "analysis": {
    "occupancyAnalysis": {
      "currentOccupancy": 45,
      "maxCapacity": 100,
      "occupancyPercentage": 45,
      "occupancyLevel": "moderate",
      "efficiencyScore": 8
    },
    "capacityAnalysis": {
      "availableSpaces": 55,
      "accessRecommendation": "allow",
      "recommendationDescription": "Space is available for new users"
    }
  }
}
```

**curl:**
```bash
curl -X POST http://localhost:5010/api/analysis/occupancy \
  -H "Content-Type: application/json" \
  -d '{
    "rawData": "{\"currentOccupancy\": 45, \"maxCapacity\": 100}",
    "includeAlerts": true
  }'
```

---

### 4. Parking Analysis
**Endpoint:** `POST /api/analysis/parking`

Analiza datos de disponibilidad de aparcamientos usando Gemini AI.

**Request Body:**
```json
{
  "rawData": "{\"totalSpots\": 50, \"occupiedSpots\": 35, \"availableSpots\": 15}",
  "parsedData": {
    "parkingSpots": [
      {"id": "P001", "status": "occupied", "is_pmr": false},
      {"id": "P002", "status": "free", "is_pmr": false},
      {"id": "P003", "status": "free", "is_pmr": true}
    ]
  }
}
```

**Response:**
```json
{
  "success": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "dataType": "PARKING_ANALYSIS",
  "analysis": {
    "occupancyAnalysis": {
      "totalSpots": 50,
      "occupiedSpots": 35,
      "availableSpots": 15,
      "occupancyPercentage": 70,
      "availabilityLevel": "medium",
      "statusDescription": "Parking availability is moderate"
    }
  }
}
```

**curl:**
```bash
curl -X POST http://localhost:5010/api/analysis/parking \
  -H "Content-Type: application/json" \
  -d '{
    "rawData": "{\"totalSpots\": 50, \"occupiedSpots\": 35}"
  }'
```

---

### 5. MCP Tools Information
**Endpoint:** `GET /api/tools/info`

Obtiene información sobre las herramientas MCP disponibles.

**Response:**
```json
{
  "status": "available",
  "timestamp": "2024-01-15T10:30:00Z",
  "tools": [
    {
      "name": "WeatherAnalysisTools",
      "description": "Analyzes meteorological data and provides weather insights",
      "methods": ["GetWeatherAnalysisWithAI"]
    },
    {
      "name": "OccupancyAnalysisTools",
      "description": "Analyzes occupancy data for spaces",
      "methods": ["GetOccupancyAnalysisWithAI"]
    },
    {
      "name": "ParkingAnalysisTools",
      "description": "Analyzes parking space availability and management",
      "methods": ["GetParkingAnalysisWithAI"]
    }
  ],
  "mcp": {
    "protocol": "HTTP Transport",
    "endpoint": "/mcp"
  }
}
```

**curl:**
```bash
curl http://localhost:5010/api/tools/info
```

---

### 6. Debug - List All Endpoints (Desarrollo)
**Endpoint:** `GET /debug/endpoints`

Lista todos los endpoints registrados en la aplicación.

**Response:**
```json
[
  {
    "route": "/api/health",
    "methods": ["GET"]
  },
  {
    "route": "/api/analysis/weather",
    "methods": ["POST"]
  },
  {
    "route": "/api/analysis/occupancy",
    "methods": ["POST"]
  },
  {
    "route": "/api/analysis/parking",
    "methods": ["POST"]
  },
  {
    "route": "/api/tools/info",
    "methods": ["GET"]
  },
  {
    "route": "/debug/endpoints",
    "methods": ["GET"]
  }
]
```

**curl:**
```bash
curl http://localhost:5010/debug/endpoints
```

---

## Modelos de Request

Se han creado tres nuevos DTOs en la carpeta `Models/`:

### WeatherAnalysisRequest.cs
```csharp
public class WeatherAnalysisRequest
{
    public required string RawData { get; set; }
    public TemperatureSensorData? ParsedData { get; set; }
    public bool? IncludeSafetyRecommendations { get; set; }
    public bool? IncludeTrendAnalysis { get; set; }
}
```

### OccupancyAnalysisRequest.cs
```csharp
public class OccupancyAnalysisRequest
{
    public required string RawData { get; set; }
    public Occupancy.OccupancySensorData? ParsedData { get; set; }
    public bool? IncludeAlerts { get; set; }
}
```

### ParkingAnalysisRequest.cs
```csharp
public class ParkingAnalysisRequest
{
    public required string RawData { get; set; }
    public ParkingSensorCollection? ParsedData { get; set; }
}
```

---

## Cómo probar los endpoints

### Con Postman/Thunder Client:
1. Abre Postman o Thunder Client
2. Crea nuevas solicitudes para cada endpoint
3. Usa los ejemplos anteriores como referencia

### Con curl:
Usa los comandos `curl` mostrados en cada sección

### Desde Visual Studio:
1. Ejecuta la aplicación (`F5`)
2. Abre la terminal integrada
3. Ejecuta los comandos curl

---

## Notas

- Todos los endpoints requieren que el servicio `GeminiAIService` esté configurado correctamente con la API Key en `appsettings.json`
- Los parámetros `ParsedData` son opcionales - si solo envías `RawData`, el servicio lo procesará
- Los parámetros booleanos (como `includeSafetyRecommendations`) tienen valores por defecto si no se especifican
- Los errores devuelven un status 400 Bad Request con detalles en la respuesta JSON
