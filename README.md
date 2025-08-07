
🎮 Ocean Blast – Match & Launch Puzzle Game
Ocean Blast, deniz temalı, eğlenceli ve tempolu bir 3D tile fırlatma puzzle oyunudur. Oyuncular, farklı renklerdeki hedef taşları fırlatarak sahadaki bloklarla eşleştirme yapar. Eşleşmeler, animasyonlarla birleşir; power-up’lar, combo’lar ve polish detayları oyun deneyimini zenginleştirir.

📦 Project Features
🎯 Core Mechanics
4 adet Launcher Box: Hedef tile’lar buraya yerleştirilir ve fırlatma burada gerçekleşir.

Grid-based Tile System: 3D objeler ile oluşturulmuş renkli tile grid yapısı.

Matching Logic: Fırlatılan tile ile grid'in ilk satırı karşılaştırılır.

Merge System: Aynı renkten 3 hedef birleşerek tek bir güçlü hedefe dönüşür.

Goal Completion: Tüm hedefler tamamlandığında seviye kazanılır.

⚙️ Architecture & Performance
Event-driven yapı (SignalBus) ile sistemler arası iletişim.

Zenject ile dependency injection.

Object Pooling:

Tile, GoalItem, Splash, Particle objeleri için özel pool sistemi.

Modüler ve SOLID prensiplere uygun mimari.

GridData ScriptableObject ile seviyeler kolayca yönetilebilir.

🔥 Game Feel & Animations
DOTween ile:

Fırlatma animasyonu (DOJump + Scale).

Merge animasyonları.

Yok olma efektleri.

Splash Effect: Fırlatma sırasında kutunun önünde çıkan efekt.

Win & Game Over UI Panel geçişleri.

Combo/Highlight Animasyonları.

Kamera Shake ve Zoom efektleri.

🛠️ Technologies Used
Unity 2022.x

C#

Zenject

DOTween

https://github.com/user-attachments/assets/37f7def0-ca7e-4e63-a296-71b9d35ec291

