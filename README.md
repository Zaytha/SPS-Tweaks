# https://zaytha.github.io/SPS-Tweaks/
make sure to update this before actually releasing the thing lol

### Adds Vertex Animation Texture support to SPS
This patcher edits sps_main.cginc to add support for [SideFXLab's Vertex Animation Textures](https://github.com/sideeffects/SideFXLabs).

This patcher doesn't do any of the VATs, just places the call to the function before SPS's bezier step. It requires a shader with VAT support to function properly, [which currently is only this modified version of poiyomi](hehe)
