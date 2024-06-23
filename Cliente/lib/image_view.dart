import 'dart:typed_data';

import 'package:flutter/material.dart';

class ImageView extends StatelessWidget {
  final Uint8List imageBytes;

  const ImageView({super.key, required this.imageBytes});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Image(
          image: MemoryImage(imageBytes),
          fit: BoxFit.fill,
        ),
      ],
    );
  }
}
