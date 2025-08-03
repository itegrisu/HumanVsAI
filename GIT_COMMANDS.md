# 🚀 Git Güncelleme Komutları

## Hızlı Güncelleme
```bash
# Değişiklikleri staging'e ekle
git add .

# Commit yap
git commit -m "✨ Yeni özellik: [özellik açıklaması]"

# GitHub'a push et
git push
```

## Örnek Commit Mesajları
```bash
# Yeni özellik
git commit -m "✨ Add achievement system with badges"

# Bug fix
git commit -m "🐛 Fix user name modal closing issue"

# UI improvements
git commit -m "💄 Improve glassmorphism effects and animations"

# Performance
git commit -m "⚡ Optimize image loading and preload system"

# Documentation
git commit -m "📝 Update README with new features"
```

## Branch Yönetimi
```bash
# Yeni feature branch
git checkout -b feature/achievement-system

# Ana branch'e dön
git checkout main

# Branch'i merge et
git merge feature/achievement-system

# Branch'i sil
git branch -d feature/achievement-system
```

## Faydalı Komutlar
```bash
# Durum kontrol
git status

# Commit geçmişi
git log --oneline

# Son commit'i geri al
git reset --soft HEAD~1

# Dosya değişikliklerini gör
git diff
```
