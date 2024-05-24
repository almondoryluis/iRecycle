import cv2

from detector import ObjectDetector, image_from_base64, image_to_base64

o = ObjectDetector()


image_filename = "./detector_test.png"
image_base64 = image_to_base64(cv2.imread(image_filename))
language_index = 1

results, image_base64 = o.detect_objects_in_image(image_base64, language_index)

for r in results:
    print(f"class_index = {r.class_index}")
    print(f"class_name = {r.class_name}")
    print(f"confidence = {r.confidence}")
    print(f"x1 = {r.x1}")
    print(f"y1 = {r.y1}")
    print(f"x2 = {r.x2}")
    print(f"y2 = {r.y2}")
    print()

image = image_from_base64(image_base64)

cv2.imshow("Detector Test", image)
cv2.waitKey(0)
cv2.destroyAllWindows()
