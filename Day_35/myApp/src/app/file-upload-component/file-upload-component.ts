import { JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { BulkInsertService } from '../services/BulkInsertService';

@Component({
  selector: 'app-file-upload-component',
  imports: [JsonPipe],
  templateUrl: './file-upload-component.html',
  styleUrl: './file-upload-component.css'
})
export class FileUploadComponent {
  constructor(private http: HttpClient) {}
  private service =  inject(BulkInsertService);
  insertedRecords:any;

  handleFileUpload(event: any) {
    const file = event.target.files[0];
    this.service.processData(file).subscribe({
      next:(data)=>this.insertedRecords= data,
      error:(err)=>alert(err)
    })
  }
}
