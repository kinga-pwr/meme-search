import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { IonicModule } from '@ionic/angular';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MemeListComponent } from './meme-list/meme-list.component';
import { MemeListItemComponent } from './meme-list-item/meme-list-item.component';
import { NgModule } from '@angular/core';
import { SearchComponent } from './search/search.component';
import { TextSearchComponent } from './text-search/text-search.component';

@NgModule({
    declarations: [
        AppComponent,
        SearchComponent,
        MemeListItemComponent,
        MemeListComponent,
        TextSearchComponent
    ],
    imports: [
        AppRoutingModule,
        BrowserAnimationsModule,
        BrowserModule,
        FlexLayoutModule,
        FormsModule,
        HttpClientModule,
        MatButtonModule,
        MatCardModule,
        MatChipsModule,
        MatGridListModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatSidenavModule,
        IonicModule.forRoot(),
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
