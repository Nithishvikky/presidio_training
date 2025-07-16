import { Component, NgModule, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentService } from '../../services/document.service';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentSharedUsersDto } from '../../models/documentSharedUsersDto';
import { CommonModule } from '@angular/common';
import { Toast } from 'bootstrap';
import { FormsModule } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { DocumentViewersDto } from '../../models/documentViewersDto';
import { DocumentViewerService } from '../../services/documentView.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { getFileTypeIcon } from '../../utility/getFileTypeIcon';

@Component({
  selector: 'app-admin-document-preview',
  imports: [CommonModule],
  templateUrl: './admin-document-preview.html',
  styleUrl: './admin-document-preview.css'
})
export class AdminDocumentPreview {
getFileTypeIcon = getFileTypeIcon;
  filename!:string;
  fileOwner:string = "";
  fileSharedUsers:DocumentSharedUsersDto[] | null = null;
  fileViewers:DocumentViewersDto[]| null = null;
  fileData:DocumentDetailsResponseDto | null = null;
  file:Blob | null = null;
  roleFlag:boolean = false;
  userEmailForGrant:string = "";
  iframeSrc: any;

  constructor(
    private route:ActivatedRoute,
    private documentService:DocumentService,
    private documentAccesService:DocumentAccessService,
    private documentViewerService:DocumentViewerService,
    private sanitizer:DomSanitizer,
    private router:Router
  ){}

  ngOnInit(): void {
    const name = this.route.snapshot.params['filename'];
    if(name){
      this.filename = name;
    }

    this.documentService.documentDetail$.subscribe({
      next:(res:any)=>{
        this.loadPreview(res);
        console.log(res);
      }
    })

    this.documentAccesService.sharedUsers$.subscribe(users =>{
        console.log(users);
        this.fileSharedUsers = users;
    })
    this.documentViewerService.viewer$.subscribe(viewers =>{
        console.log(viewers);
        this.fileViewers = viewers;
    })
  }

  loadPreview(res:any){
    console.log(res);
    this.fileData = res;
    const byteCharacters = atob(res.fileData);
    const byteNumbers = new Array(byteCharacters.length);
    for(let i=0;i<byteCharacters.length;i++){
      byteNumbers[i]=byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray],{type: res.contentType});
    this.file = blob;
    const fileURL = URL.createObjectURL(blob);
    this.iframeSrc = this.sanitizer.bypassSecurityTrustResourceUrl(fileURL);
    console.log(`${this.filename}:${this.iframeSrc}`);
  }

  onDownload(){
    if(!this.file){
      this.showToast("Can't load the file","danger");
      return;
    }

    const url = URL.createObjectURL(this.file);
    const a = document.createElement('a');
    a.href = url;

    if(!this.fileData) return;

    a.download = this.fileData?.fileName;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);

    this.showToast("File downloaded sucessfully!","success");
  }

  onDelete(){
    if(this.fileData){
      this.documentService.AdminDeleteDocument(this.fileData.fileName,this.fileData.uploaderEmail).subscribe({
        next:(res) =>{
          console.log(res);
          this.showToast("File Deleted Succefully","danger");
        },
        error:(err)=>{
          console.error(err);
          this.showToast(err.error.error.errorMessage,"danger");
        }
      })
      this.router.navigate(['/main/alldocuments']);
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
