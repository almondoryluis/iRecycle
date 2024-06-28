import base64

import cv2
import numpy as np
from PIL import Image, ImageDraw, ImageFont
from ultralytics import YOLO
from ultralytics.engine.results import Results
from ultralytics.nn.tasks import DetectionModel


def positive_int(number) -> int:
    n = int(number)
    if n < 0:
        n = 0
    return n


def image_from_base64(image_base64: str) -> cv2.typing.MatLike:
    numpy_array = np.fromstring(base64.b64decode(image_base64), np.uint8)
    return cv2.imdecode(numpy_array, cv2.IMREAD_ANYCOLOR)


def image_to_base64(image: cv2.typing.MatLike) -> str:
    _, buffer = cv2.imencode(".jpg", image)
    return base64.b64encode(buffer.tobytes()).decode("utf-8")


class DetectedObject:
    class_index: int
    class_name: str
    confidence: int
    x1: int
    y1: int
    x2: int
    y2: int

    def __init__(
        self,
        class_index: int,
        class_name: str,
        confidence: int,
        x1: int,
        y1: int,
        x2: int,
        y2: int,
    ):
        self.class_index = class_index
        self.class_name = class_name
        self.confidence = confidence
        self.x1 = x1
        self.y1 = y1
        self.x2 = x2
        self.y2 = y2


class ObjectDetector:
    font_filename = "./Roboto/Roboto-Regular.ttf"
    font_stroke_width = 1
    font_size = 24
    font: ImageFont.FreeTypeFont
    model_filename = "./Modelo/best.pt"
    model: DetectionModel

    labels_en = ["Metal", "Glass", "Plastic", "Cardboard", "Medical"]
    labels_es = ["Metal", "Vidrio", "Plástico", "Cartón", "Residuo médico"]

    labels = [labels_en, labels_es]

    colors = [(0, 0, 255), (255, 0, 0), (255, 255, 0), (0, 255, 255), (255, 0, 255)]

    border_width = 4
    padding = 8

    def __init__(self) -> None:
        self.font = ImageFont.truetype(font=self.font_filename, size=self.font_size)
        self.model = YOLO(self.model_filename)

    def detect_objects_in_image(
        self, input_image_base64: str, language_index: int
    ) -> tuple[list[DetectedObject], str]:
        image = image_from_base64(input_image_base64)
        results: list[Results] = self.model(image, stream=False, verbose=False)
        detected_objects: list[DetectedObject] = []

        pil_image = Image.fromarray(image)
        pil_draw = ImageDraw.Draw(pil_image)

        for result in results:
            for box in result.boxes:
                x1, y1, x2, y2 = box.xyxy[0]
                x1, y1, x2, y2 = (
                    positive_int(x1),
                    positive_int(y1),
                    positive_int(x2),
                    positive_int(y2),
                )
                class_index = int(box.cls[0])
                confidence = int(box.conf[0] * 100.0)
                label = self.labels[language_index][class_index]
                border_color = self.colors[class_index]

                box1 = [x1, y1, x2, y2]
                pil_draw.rounded_rectangle(
                    xy=box1, radius=10, outline=border_color, width=self.border_width
                )

                (a, b, c, d) = pil_draw.textbbox(
                    [x1, y1],
                    label,
                    font=self.font,
                    anchor="lb",
                )
                a -= self.padding
                b -= self.padding
                c += self.padding
                d += self.padding
                box2 = (a, b, c, d)

                pil_draw.rounded_rectangle(
                    xy=box2,
                    radius=4,
                    fill=(0, 0, 0),
                    outline=border_color,
                    width=self.border_width,
                )
                pil_draw.text(
                    [x1, y1],
                    label,
                    font=self.font,
                    anchor="lb",
                    fill=(255, 255, 255),
                )

                detected_objects.append(
                    DetectedObject(
                        class_index=class_index,
                        class_name=label,
                        confidence=confidence,
                        x1=x1,
                        y1=y1,
                        x2=x2,
                        y2=y2,
                    )
                )

        image = np.array(pil_image)
        output_image_base64 = image_to_base64(image)

        return (detected_objects, output_image_base64)
