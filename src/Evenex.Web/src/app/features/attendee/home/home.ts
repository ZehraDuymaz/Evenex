import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Header } from '../../../shared/components/header/header';

interface EventData {
  id: string;
  title: string;
  date: string;
  location: string;
  price: number;
  imageUrl: string;
  category: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, Header],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit, OnDestroy {
  categories = ['Tümü', 'Müzik', 'Sahne', 'Spor', 'Aile'];
  activeCategory = 'Tümü';
  currentHeroIndex = 0;
  private autoSlideInterval: any;

  popularEvents: EventData[] = [
    {
      id: '1',
      title: 'Yalın - Anadolu Turnesi',
      date: '15 Eki 2026 • 20:00',
      location: 'Harbiye Cemil Topuzlu Açıkhava',
      price: 450,
      imageUrl: 'https://images.unsplash.com/photo-1459749411175-04bf5292ceea?auto=format&fit=crop&q=80&w=800',
      category: 'Müzik'
    },
    {
      id: '2',
      title: 'Adamlar',
      date: '02 Kas 2026 • 21:00',
      location: 'Jolly Joker Ankara',
      price: 250,
      imageUrl: 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&q=80&w=800',
      category: 'Müzik'
    },
    {
      id: '3',
      title: 'Salih Bademci - Sesler',
      date: '20 Eki 2026 • 19:30',
      location: 'Zorlu PSM',
      price: 350,
      imageUrl: 'https://images.unsplash.com/photo-1459749411175-04bf5292ceea?auto=format&fit=crop&q=80&w=800',
      category: 'Sahne'
    },
    {
      id: '4',
      title: 'Fenerbahçe Beko - Anadolu Efes',
      date: '10 Kas 2026 • 19:00',
      location: 'Ülker Spor ve Etkinlik Salonu',
      price: 150,
      imageUrl: 'https://images.unsplash.com/photo-1552674605-db6ffd4facb5?auto=format&fit=crop&q=80&w=800',
      category: 'Spor'
    },
    {
      id: '5',
      title: 'Emir Can İğrek',
      date: '30 Eyl 2026 • 21:00',
      location: 'Kültürpark Açıkhava Tiyatrosu',
      price: 300,
      imageUrl: 'https://images.unsplash.com/photo-1415201364774-f6f0bb35f28f?auto=format&fit=crop&q=80&w=800',
      category: 'Müzik'
    },
    {
      id: '6',
      title: 'Ebru Yaşar & Senfoni Orkestrası',
      date: '05 Ara 2026 • 20:30',
      location: 'Bostancı Gösteri Merkezi',
      price: 500,
      imageUrl: 'https://images.unsplash.com/photo-1465847899084-d164df4dedc6?q=80&w=1470&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
      category: 'Müzik'
    }
  ];

  get filteredEvents() {
    if (this.activeCategory === 'Tümü') return this.popularEvents;
    return this.popularEvents.filter(e => e.category === this.activeCategory);
  }

  setCategory(cat: string) {
    this.activeCategory = cat;
  }

  ngOnInit() {
    this.startAutoSlide();
  }

  ngOnDestroy() {
    this.stopAutoSlide();
  }

  startAutoSlide() {
    this.autoSlideInterval = setInterval(() => {
      this.nextHero();
    }, 3000);
  }

  stopAutoSlide() {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  nextHero() {
    this.currentHeroIndex = (this.currentHeroIndex + 1) % this.popularEvents.length;
  }

  prevHero() {
    this.currentHeroIndex = (this.currentHeroIndex - 1 + this.popularEvents.length) % this.popularEvents.length;
  }

  onUserSlide(direction: 'next' | 'prev') {
    this.stopAutoSlide();
    if (direction === 'next') this.nextHero();
    else this.prevHero();
    this.startAutoSlide();
  }
}
