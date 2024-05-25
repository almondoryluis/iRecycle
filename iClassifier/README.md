# iClassifier

Servicio de clasificación de objetos mediante IA.

Los objetos clasificados pueden ser de las siguientes clases:

| class_index | class_name |
|-------------|------------|
| 0           | metal      |
| 1           | glass      |
| 2           | plastic    |
| 3           | cardboard  |
| 4           | medical    |

## Endpoint

~~~
POST /classify
~~~

Permite realizar la clasificación de objetos.

### Entrada

La entrada es un texto de tipo `application/json` con la siguiente estructura:

~~~typescript
{
    image_base64: string,  // imagen de entrada, en Base64, con los objetos a clasificar
    language_index: int    // idioma para etiquetar los objetos clasificados
                           // donde [0 = English, 1 = Español]
}
~~~

### Salida

La salida es un texto de tipo `application/json` con la siguiente estructura:

~~~typescript
{
    detected: bool,         // detected es true si se detectó y clasificó al menos un objeto, false en otro caso
    error_message: string,  // error_message es "" si no hubo error
    detected_objects: ObjectData[],  // un vector de objetos detectados y clasificados, ver ObjectData más abajo
    image_base64: string             // la imagen de entrada con los objetos etiquetados
}
~~~

Donde los elementos `ObjectData` tienen la siguiente estructura:

~~~typescript
{
    class_index: int,    // índice de la clase del objeto detectado,
                         // donde [0 = 'metal', 1 = 'glass', 2 = 'plastic', 3 = 'cardboard', 4 = 'medical']
    class_name: string,  // nombre de la clase (etiqueta) del objeto detectado,
                         // según el idioma seleccionado por language_index en la Entrada
    confidence: int,     // entero del rango [0, 100] que indica el nivel de confianza de la clasificación realizada
    x1: int,  // coordenada x de la esquina superior izquierda del rectángulo que enmarca el objeto clasificado
    y1: int,  // coordenada y de la esquina superior izquierda del rectángulo que enmarca el objeto clasificado
    x2: int,  // coordenada x de la esquina inferior derecha del rectángulo que enmarca el objeto clasificado
    y2: int   // coordenada x de la esquina inferior derecha del rectángulo que enmarca el objeto clasificado
}
~~~

**NOTAS**:
1. La Salida de este endpoint es válida
   si y sólo si se recibe el código de estado HTTP 200 OK.
2. Si este endpoint atrapa alguna excepción en el proceso de clasificacíon,
   también devuelve una respuesta (Salida) con el código de estado HTTP 200 OK
   y el respectivo mensaje de error en el campo `error_message` de la Salida.
3. Si ocurre alguna excepción no atrapada por este endpoint,
   el framework utilizado (FastAPI) devuelve una respuesta
   con un código de estado HTTP diferente de 200
   y un texto de tipo `application/json` con datos sobre el error.
   La estructura de este texto JSON varía según el tipo de error.
   Para mayor información sobre las respuestas del framework FastAPI,
   en caso de una excepción no atrapada por este endpoint, consultar:
   * https://fastapi.tiangolo.com/reference/exceptions/#fastapi.HTTPException
   * https://fastapi.tiangolo.com/tutorial/handling-errors/

## Modelo

El modelo utilizado para realizar la clasificación fue obtenido de https://huggingface.co/AprendeIngenia/recyclingAI.

Referencias:
* https://github.com/AprendeIngenia/reciclaje_ai
* https://youtu.be/PSFw_ipK3ug
