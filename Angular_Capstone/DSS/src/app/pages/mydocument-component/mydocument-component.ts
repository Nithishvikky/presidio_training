import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Modal, Toast } from 'bootstrap';
import { DocumentService } from '../../services/document.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';


@Component({
  selector: 'app-mydocument-component',
  imports: [CommonModule,FormsModule,RouterModule],
  templateUrl: './mydocument-component.html',
  styleUrl: './mydocument-component.css'
})
export class MydocumentComponent {
  documents: DocumentDetailsResponseDto[] | null = null;
  selectedFile: File|null = null;
  fileSizeFlag:boolean = false;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  constructor(private documentService:DocumentService){}

  ngOnInit():void{
    this.documentService.documents$.subscribe(doc =>{
      this.documents = doc;
    })
    this.documentService.GetAllDocuments().subscribe();
  }

  loadDocuments(){
    this.documentService.GetAllDocuments().subscribe({
      next:(res:any)=>{
        console.log(res);
        this.documents = res.data.$values;
      },
      error:(err)=> {
        console.log(err);
      },
    })
  }

  onFileSelected(e: Event):void{
    const input = e.target as HTMLInputElement;
    if (input?.files && input.files.length > 0) {
      let sizeInMB = (input.files[0].size / (1024 * 1024));
      if(sizeInMB < 10){
        this.selectedFile = input.files[0];
        this.fileSizeFlag = false;
      }
      else{
        this.fileSizeFlag = true;
      }
    }
  }

  onFileDelete(filename:string){
    this.documentService.DeleteDocument(filename).subscribe({
      next:(res) =>{
        console.log(filename);
        this.showToast("File Deleted Succefully","danger");
      },
      error:(err)=>{
        console.error(err);
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  onUpload():void{
    if(!this.selectedFile) return;

    const formData = new FormData();
    formData.append('file',this.selectedFile);

    this.documentService.PostDocument(formData).subscribe({
      next:(res) =>{
        console.log(res);
        this.onUploadCancel();
        this.showToast("File Uploaded Succefully","success");
      },
      error:(err)=>{
        console.error(err);
        this.onUploadCancel();
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  onUploadCancel():void{
    this.selectedFile = null;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  showToast(message: string, type: 'success' | 'danger') {
      const toastEl = document.getElementById('liveToast');
      const toastBody = document.querySelector('.toast-body');

      toastBody!.textContent = message;
      toastEl!.classList.remove('bg-success', 'bg-danger');
      toastEl!.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

      const toast = new Toast(toastEl!);
      toast.show();
  }
}
