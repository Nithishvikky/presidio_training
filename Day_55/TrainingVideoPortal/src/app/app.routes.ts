import { Routes } from '@angular/router';
import { VideoUploadComponent } from './components/video-upload-component/video-upload-component';
import { VideoListComponent } from './components/video-list-component/video-list-component';

export const routes: Routes = [
  { path: 'upload', component: VideoUploadComponent },
  { path: '', component: VideoListComponent },
  { path: '**', redirectTo: '' }
];
