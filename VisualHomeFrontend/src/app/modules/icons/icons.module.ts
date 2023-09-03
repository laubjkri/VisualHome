import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MatIconRegistry } from '@angular/material/icon';


@NgModule({
  declarations: [],
  imports: [
    CommonModule    
  ]
})
export class IconsModule { 
  private readonly path: string = "assets/icons/";

  constructor(
    private domSanitizer: DomSanitizer,
    public matIconRegistry: MatIconRegistry
  )
  {    
    this.matIconRegistry.addSvgIcon("key", this.setPath(this.path + "key.svg"));    
    this.matIconRegistry.addSvgIcon("manage_accounts", this.setPath(this.path + "manage_accounts.svg"));
    this.matIconRegistry.addSvgIcon("dashboard", this.setPath(this.path + "dashboard.svg"));
  }

  private setPath(url: string): SafeResourceUrl {
    return this.domSanitizer.bypassSecurityTrustResourceUrl(url);
  }
}
