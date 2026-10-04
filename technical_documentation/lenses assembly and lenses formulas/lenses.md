# DeltaVR - dual fresnel optics

> [!summary] summary
> d.70 pmma fresnels, **2 mm** thick each. stack is **f40 (display) + f30 (eye)**, grooves side meeting at the middle seperated by the gasket (*0.2mm*)
> combined efl **≈ 17.2 mm**. display gap is **15.0 mm**, eye relief is **10 mm**.

**parts:** d70 fl30 (top) + d70 fl40(bottom), 5 € ish each also buy spares. 

**cut outline:** see `deltavr-lens-cut.png` (optical center on pupil, nose bite, mirror L/R). trim does **not** change focal length (fl).

---

## 1. layout

```
display active surface
   │
   |  15.0 mm air          <-- dist. to disp. from len not holder, thats 14mm
   ▼
   f40 fresnel  2 mm       flat side toward display, grooves inward
   │
   │  0.2 mm spacer        printed as first layer (ring, not take up fov too much)
   ▼
   f30 fresnel  2 mm       grooves inward, flat toward eye <- top is more focused
   │
   │  10 mm air            eye relief (12–15 if glasses)
   ▼
cornea of eye
```

| gap                   | value        | notes                                                           |
| --------------------- | ------------ | --------------------------------------------------------------- |
| display -> f40 flat   | **15.0 mm**  | caliper this. 14 = near focus, 15 = infinity-ish, 16 = far bias |
| f40 thickness         | **2.0 mm**   | catalog, cat. shouldnt change                                   |
| between lenses        | **0.2 mm**   | grooves face each other. ring spacer, not a disc                |
| f30 thickness         | **2.0 mm**   | catalog                                                         |
| f30 flat -> eye       | **10 mm**    | Min. 12–15 mm for glasses                                       |
| **optical module**    | **19.2 mm**  | display face → outer face of eye lens. cylinder depth           |
| **display -> cornea** | **~29.2 mm** | includes eye-relief air. do **not** put this in the barrel      |

---

## 2. sign convention

real-is-positive, thin-lens form:

$$\frac{1}{f} = \frac{1}{d_o} + \frac{1}{d_i}$$

virtual image (eyepiece case) → $d_i < 0$.

---

## 3. single lens

| symbol | meaning | value |
|---|---|---|
| $f$ | focal length (efl) | 30 mm or 40 mm |
| $d_o$ | object distance (display → lens) | solve |
| $d_i$ | image distance (lens → image) | $-\infty$ or $-250\ \mathrm{mm}$ |
| $t$ | substrate thickness | **2.0 mm** |
| $n$ | pmma index | 1.49 (catalog fl already includes this) |

> [!note] fresnel fl reference
> catalog fl is measured from the **grooved surface**. the 2 mm blank is mechanical dead plastic; power lives on the grooves. that is why $g_{\mathrm{disp}} = \mathrm{FFL} - t$.

---

## 4. two thin lenses, separation $d$

groove-to-groove: $d = 0.2\ \mathrm{mm}$.

$f_1 = 40$ (display side), $f_2 = 30$ (eye side).

### equivalent focal length

$$\frac{1}{f_{\mathrm{eq}}} = \frac{1}{f_1} + \frac{1}{f_2} - \frac{d}{f_1 f_2}$$

$$\frac{1}{f_{\mathrm{eq}}} = \frac{1}{40} + \frac{1}{30} - \frac{0.2}{40 \cdot 30} = 0.025 + 0.033333 - 0.000167 = 0.058167$$

$$\boxed{f_{\mathrm{eq}} = 17.19\ \mathrm{mm}}$$

contact limit ($d \approx 0$):

$$f = \frac{f_1 f_2}{f_1 + f_2} = \frac{1200}{70} = 17.14\ \mathrm{mm}$$

---

## 5. front focal length (display placement)

object at front focal point → image at infinity (relaxed eye). ffl is measured from the **first lens groove plane** ($L_1$ = f40).

$$\mathrm{FFL} = f_{\mathrm{eq}}\left(1 - \frac{d}{f_2}\right) = 17.19 \times 0.99333 = 17.08\ \mathrm{mm}$$

principal-plane offset from $L_1$ is $\bar{H_1} = -f_{\mathrm{eq}}\, d / f_2 \approx -0.12\ \mathrm{mm}$. negligible. treat the f40 groove plane as the front reference.

**mechanical gap (flat → display):**

$$\boxed{g_{\mathrm{disp}} = \mathrm{FFL} - t_1 = 17.08 - 2.0 = 15.1\ \mathrm{mm} \approx 15.0\ \mathrm{mm}}$$

---

## 6. focus target (not just infinity)

$$\frac{1}{d_o} = \frac{1}{f_{\mathrm{eq}}} - \frac{1}{d_i}, \qquad d_i = -|d_i|\ \text{(virtual)}$$

$$d_o = \left(\frac{1}{f_{\mathrm{eq}}} + \frac{1}{|d_i|}\right)^{-1}$$

$$\boxed{g_{\mathrm{disp}}(d_i) = \left(\frac{1}{f_{\mathrm{eq}}} + \frac{1}{|d_i|}\right)^{-1} - t_1}$$

| target image | $\|d_i\|$ | $d_o$ (to groove) | $g_{\mathrm{disp}} = d_o - t_1$ |
|---|---|---|---|
| infinity (relaxed) | $\infty$ | 17.1 mm | **15.1 mm** |
| 2 m (far) | 2000 | 16.4 mm | 14.4 mm |
| 25 cm (reading) | 250 | 16.1 mm | **14.1 mm** |
| 10 cm (too close) | 100 | 14.4 mm | 12.4 mm |
| 6 cm (why 13.2 hurts) | 58 | 13.2 mm | 11.2 mm |

> [!warning] old 13.2 mm gap
> manolo's measured 13.2 mm doesnt match 30+40 thin lens math. it puts the virtual image at ~6 cm and eyes will hate it. if a housing is already cut at 13.2, print a ~2 mm shim. design point is **15.0 mm**, shim range **14–16 mm**.

---

## 7. eye side

firstly, eye relief is mechanical not optical:

$$g_{\mathrm{eye}} = \mathrm{cornea} - \mathrm{f30\ flat}$$

| use               | $g_{\mathrm{eye}}$ |
| ----------------- | ------------------ |
| lashes / max fov  | 8–10 mm            |
| no-glasses design | 10 mm              |
| glasses           | 12–15 mm           |

vignetting eats fov when relief is large vs d.70 clear aperture. 10 mm is tight already.

---

## 8. full stack length

### optical module

ends at the outer face of the eye lens, **not** at the cornea.

$$L_{\mathrm{mod}} = g_{\mathrm{disp}} + t_1 + d + t_2$$

$$L_{\mathrm{mod}} = 15.0 + 2.0 + 0.2 + 2.0 = \mathbf{19.2\ \mathrm{mm}}$$

### display → cornea

$$\boxed{L_{\mathrm{total}} = g_{\mathrm{disp}} + t_1 + d + t_2 + g_{\mathrm{eye}} = 15.0 + 2.0 + 0.2 + 2.0 + 10.0 = 29.2\ \mathrm{mm}}$$

comfort ($g_{\mathrm{eye}} = 13.2$): $32.4\ \mathrm{mm}$.

### housing front-back (mechanical)

$$W_{\mathrm{FB}} = t_{\mathrm{display\ PCB}} + t_{\mathrm{cover}} + L_{\mathrm{mod}} + g_{\mathrm{eye}} + t_{\mathrm{face\ gasket}}$$

$$\approx 5 + 1 + 19.2 + 10 + 4 \approx \mathbf{39\ \mathrm{mm}} \quad (\sim 35\ \mathrm{mm}\ \text{if pcb+gasket squeezed})$$

> [!danger] dont double count
> do not put eye relief inside the optical cylinder. module is 19.2 mm. the 10 mm of air in front of the eye is outside the barrel.

---

## 9. fov

panel is the LS029B3SX02, active $w = 26.6\ \mathrm{mm}$, half-width $h = 13.3\ \mathrm{mm}$.

### ideal

$$\mathrm{FOV}_{1\mathrm{D}} = 2 \arctan\left(\frac{h}{f_{\mathrm{eq}}}\right) = 2 \arctan\left(\frac{13.3}{17.19}\right) = \mathbf{75.4°}$$

diagonal (square panel, half-diag $= h\sqrt{2}$):

$$\mathrm{FOV}_{\mathrm{diag}} = 2 \arctan\left(\frac{h\sqrt{2}}{f_{\mathrm{eq}}}\right) = 2 \arctan\left(\frac{18.81}{17.19}\right) = \mathbf{95.2°}$$

general:

$$\boxed{\mathrm{FOV} = 2 \arctan\left(\frac{d_{\mathrm{sensor}}}{2\, f_{\mathrm{eq}}}\right)}$$

### real (relief + diameter losses)

$$\mathrm{FOV}_{\mathrm{real}} = 2 \arctan\left(\frac{\min(h,\ r_{\mathrm{clear}})}{g_{\mathrm{eye}} + f_{\mathrm{eq}}}\right) \cdot k_{\mathrm{eyebox}}$$

expect **5–10° less** on the diagonal once relief and ø70 fight you. still quest 2-class (~75° / ~95° ideal).

---

## 10. alternate stack (two f30s)

only real shrink lever. same blank, same cutout, one part number.

$$f_{\mathrm{eq}} = \frac{30 \cdot 30}{30 + 30} = 15.0\ \mathrm{mm}$$

$$g_{\mathrm{disp}} = 15.0 - 2.0 = 13.0\ \mathrm{mm}$$

$$L_{\mathrm{total}} = 13 + 2 + 0.2 + 2 + 10 = 27.2\ \mathrm{mm}$$

$$\mathrm{FOV}_{1\mathrm{D}} = 2\arctan(13.3/15) = 82.0°$$

| stack | efl | display gap | display→cornea | fov / axis |
|---|---|---|---|---|
| **f40 + f30** (current) | 17.2 mm | 15.0 mm | 29.2 mm | 75° |
| f30 + f30 | 15.0 mm | 13.0 mm | 27.2 mm | 82° |

---

## 11. diameter (keep ø70)

matching diameters with **both** fl30 and fl40 in the listing:

| diameter | fl30 | fl40 | both? |
|---|---|---|---|
| **d70** | yes | **yes** | **only option** |
| d90 | yes | (fl50) | no |
| d100 | | yes | no |
| d60 | yes | (fl80) | no |
| d50 / d40 | no | no | no |

d100 fl120 at 10 € is a trap. cutout work stays valid on ø70.

---

## 12. variable sheet

| variable | symbol | formula | value |
|---|---|---|---|
| display-side fl | $f_1$ | catalog | 40 mm |
| eye-side fl | $f_2$ | catalog | 30 mm |
| lens thickness | $t_1, t_2$ | catalog | 2.0 mm each |
| spacer | $d$ | first layer | 0.2 mm |
| combined efl | $f_{\mathrm{eq}}$ | $\left(\frac{1}{f_1}+\frac{1}{f_2}-\frac{d}{f_1 f_2}\right)^{-1}$ | **17.19 mm** |
| front focal length | ffl | $f_{\mathrm{eq}}(1 - d/f_2)$ | 17.08 mm |
| display gap (mech) | $g_{\mathrm{disp}}$ | $\mathrm{FFL} - t_1$ | **15.0 mm** |
| focus target | $d_i$ | design choice | $\infty$ or $-250\ \mathrm{mm}$ |
| object distance | $d_o$ | $\left(\frac{1}{f_{\mathrm{eq}}}+\frac{1}{\|d_i\|}\right)^{-1}$ | 17.1 / 16.1 mm |
| eye relief | $g_{\mathrm{eye}}$ | mechanical | **10 mm** |
| optical module | $L_{\mathrm{mod}}$ | $g_{\mathrm{disp}}+t_1+d+t_2$ | **19.2 mm** |
| display→cornea | $L_{\mathrm{total}}$ | $L_{\mathrm{mod}}+g_{\mathrm{eye}}$ | **29.2 mm** |
| half panel | $h$ | $w/2$ | 13.3 mm |
| fov / axis | $\mathrm{FOV}_{1\mathrm{D}}$ | $2\arctan(h/f_{\mathrm{eq}})$ | **75°** |
| fov diagonal | $\mathrm{FOV}_{\mathrm{diag}}$ | $2\arctan(h\sqrt{2}/f_{\mathrm{eq}})$ | **95°** |
| diameter | $D$ | keep cutout | 70 mm |
| ior | $n$ | pmma | 1.49 |

---

## 13. what is *not* in the math

- **$t = 2\ \mathrm{mm}$ does not change $f$.** fresnel power is the groove prism angles. thickness only enters as $g_{\mathrm{disp}} = \mathrm{FFL} - t$ because catalog fl is from the grooves.
- **cutting the outline does not change $f$.** trim is mechanical only.
- **thick-lens principal-plane shift** $\bar{H} \sim t/n \approx 1.3\ \mathrm{mm}$ matters for bulk lenses. for a fresnel with all power at one face, the principal plane sits at the grooves. already handled.
- **seidel aberrations, distortion, astigmatism.** not first-order. the pair (stronger toward the eye) trims some spherical aberration vs a single element; it does not change the first-order numbers above.

---

## 14. build rules

1. optical center (groove center) stays on the pupil.
2. grooves meet in the middle; flats face display and eye.
3. f30 (shorter fl, "more focused") goes toward the **eye**. f40 toward the **display**.
4. spacer is a ø70 **ring** (od 70, id = clear aperture). nothing plastic over the optical zone.
5. no soldering iron on faces. acrylic cement on the outer rim only, or a mechanical ring cassette. heat warps grooves and the lens is dead.
6. mirror the cut outline for left/right. hand-trim is the known-good path.
7. shim the display gap. 15.0 mm nominal, 14–16 mm is the useful window.

---

## 15. summary

$$f_{\mathrm{eq}} = 17.2\ \mathrm{mm} \implies g_{\mathrm{disp}} = f_{\mathrm{eq}} - 2 = 15.0\ \mathrm{mm}$$

$$L_{\mathrm{total}} = 15+2+0.2+2+10 = 29.2\ \mathrm{mm}, \quad \mathrm{FOV} \approx 75°/95°$$

---

*deltavr optics · 2 mm pmma · ø70 · f40+f30 · spacer 0.2 mm*
