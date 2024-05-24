from fastapi import FastAPI
from pydantic import BaseModel

from detector import ObjectDetector


class InputData(BaseModel):
    image_base64: str
    language_index: int


class ObjectData(BaseModel):
    class_index: int
    class_name: str
    confidence: int
    x1: int
    y1: int
    x2: int
    y2: int


class OutputData(BaseModel):
    detected: bool
    error_message: str
    detected_objects: list[ObjectData]
    image_base64: str


print("Loading detector...")
detector = ObjectDetector()
print("OK")

app = FastAPI()


@app.post("/classify")
def classify_objects(input: InputData) -> OutputData:
    try:
        print("Detecting objects...")
        objects, image_base64 = detector.detect_objects_in_image(
            input.image_base64, input.language_index
        )
        print("OK")
        detected_objects: list[ObjectData] = []
        for o in objects:
            detected_objects.append(
                ObjectData(
                    class_index=o.class_index,
                    class_name=o.class_name,
                    confidence=o.confidence,
                    x1=o.x1,
                    y1=o.y1,
                    x2=o.x2,
                    y2=o.y2,
                )
            )

        if len(detected_objects) > 0:
            return OutputData(
                detected=True,
                error_message="",
                image_base64=image_base64,
                detected_objects=detected_objects,
            )
        else:
            return OutputData(
                detected=False,
                error_message="",
                image_base64="",
                detected_objects=[],
            )
    except BaseException as error:
        return OutputData(
            detected=False,
            error_message=f"{error}",
            image_base64=image_base64,
            detected_objects=[],
        )
