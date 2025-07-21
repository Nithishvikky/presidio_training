import { Component } from '@angular/core';
import { VideoService,TrainingVideo } from '../../services/video.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-video-list-component',
  imports: [CommonModule],
  templateUrl: './video-list-component.html',
  styleUrl: './video-list-component.css'
})
export class VideoListComponent {
  videos: TrainingVideo[] = [];

  constructor(private videoService: VideoService) {}

  ngOnInit(): void {
    this.videoService.getVideos().subscribe({
      next: (res) => {
        this.videos = res;
      },
      error: (err) => {
        console.error('Failed to load videos', err);
      }
    });
  }
}
