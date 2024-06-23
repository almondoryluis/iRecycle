import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:camera/camera.dart';
import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:irecycle/image_view.dart';

late List<CameraDescription> _cameras;
const ResolutionPreset cameraResolutionPreset = ResolutionPreset.low;

final dio = Dio(BaseOptions(
  baseUrl: 'http://192.168.0.86:8000',
  connectTimeout: const Duration(seconds: 5),
  receiveTimeout: const Duration(seconds: 60 * 3),
));

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  _cameras = await availableCameras();
  runApp(
    const MaterialApp(
      home: Scaffold(
        body: CameraApp(),
        extendBody: true,
      ),
    ),
  );
}

/// CameraApp is the Main Application.
class CameraApp extends StatefulWidget {
  /// Default Constructor
  const CameraApp({super.key});

  @override
  State<CameraApp> createState() => _CameraAppState();
}

class _CameraAppState extends State<CameraApp> {
  late CameraController cameraController;
  late final AppLifecycleListener listener;
  bool detectingObjects = false;

  @override
  void initState() {
    super.initState();
    cameraController = CameraController(_cameras[0], cameraResolutionPreset);
    cameraController.initialize().then((_) {
      if (!mounted) {
        return;
      }
      setState(() {});
    }).catchError((Object e) {
      if (e is CameraException) {
        switch (e.code) {
          case 'CameraAccessDenied':
            // Handle access errors here.
            break;
          default:
            // Handle other errors here.
            break;
        }
      }
    });
  }

  @override
  void dispose() {
    cameraController.dispose();
    super.dispose();
  }

  void tomarFoto() async {
    var image = await cameraController.takePicture();

    if (mounted) {
      Uint8List inputImageBytes = await image.readAsBytes();
      await File(image.path).delete();

      String inputImageBase64 = base64Encode(inputImageBytes);

      var snackBar = ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Detectando objetos...'),
          duration: Duration(minutes: 3),
        ),
      );

      setState(() {
        detectingObjects = true;
      });

      var response = await dio.post(
        '/classify',
        data: {'image_base64': inputImageBase64, 'language_index': 1},
        options: Options(responseType: ResponseType.json),
      );

      snackBar.close();
      setState(() {
        detectingObjects = false;
      });

      bool detected = response.data["detected"];

      if (detected) {
        String outputImageBase64 = response.data["image_base64"];
        debugPrint("");
        debugPrint("image_base64:");
        debugPrint(outputImageBase64);
        debugPrint("");
        Uint8List outputImageBytes = base64Decode(outputImageBase64);
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => ImageView(imageBytes: outputImageBytes),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('No se detectó ningún objeto.'),
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!cameraController.value.isInitialized) {
      return Container();
    }
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        CameraPreview(
          cameraController,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              ElevatedButton(
                onPressed: detectingObjects ? null : tomarFoto,
                child: const Text("Detectar objetos"),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
