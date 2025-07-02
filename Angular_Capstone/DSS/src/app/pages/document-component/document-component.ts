import { Component, NgModule, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

@Component({
  selector: 'app-document-component',
  imports: [CommonModule,FormsModule],
  templateUrl: './document-component.html',
  styleUrl: './document-component.css'
})
export class DocumentComponent implements OnInit{
  filename!:string;
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
    private sanitizer:DomSanitizer
  ){}

  ngOnInit(): void {
    const name = this.route.snapshot.params['filename'];
    if(name){
      this.filename = name;
    }

    const authdata = localStorage.getItem("authData");
    if(authdata){
      if(JSON.parse(authdata).role === "User"){
        this.roleFlag = true;
      }
    }

    if(!this.roleFlag){ // Admin
      this.documentService.OwnerDocumentPreview(this.filename).subscribe({
        next:(res:any)=>{
          this.loadPreview(res.data);
        }})

      this.documentAccesService.sharedUsers$.subscribe(users =>{
        console.log(users);
        this.fileSharedUsers = users;
      })
      this.documentAccesService.GetSharedUsers(this.filename).subscribe();

      this.documentViewerService.viewer$.subscribe(viewers =>{
        this.fileViewers = viewers;
      })
      this.documentViewerService.GetViewerofFile(this.filename).subscribe();
    }
    else{
      this.documentService.documentDetail$.subscribe({
        next:(res:any)=>{
          this.loadPreview(res);
        }
      })
    }
  }

  GrantPermissionToUser(){
    if(!this.userEmailForGrant) return this.showToast("Enter valid Email","danger");

    this.documentAccesService.GrantPermissionToUser(this.filename,this.userEmailForGrant).subscribe({
      next:(res:any)=>{
        this.showToast("Permission Granted","success");
      },
      error:(err)=>{
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
    this.userEmailForGrant = "";
  }

  RevokePermissionToUser(email:string){
    this.documentAccesService.RevokePermissionToUser(this.filename,email).subscribe({
      next:(res:any)=>{
        this.showToast(`Permission Revoked for ${email}`,"danger");
      },
      error:(err)=>{
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  GrantPermissionForAll(){
    this.documentAccesService.GrantPermissionToAll(this.filename).subscribe({
      next:(res:any)=>{
        this.showToast("Permission Granted for all","success");
      },
      error:(err)=>{
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  RevokePermissionForAll(){
    this.documentAccesService.RevokePermissionToAll(this.filename).subscribe({
      next:(res:any)=>{
        this.showToast("Permission Revoked for all","danger");
      },
      error:(err)=>{
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  loadPreview(res:any){
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
