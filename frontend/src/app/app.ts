import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html'
})
export class App implements OnInit {
  produtos: any[] = [];
  notas: any[] = [];
  
  novoProduto = { descricao: '', saldo: 0 };
  novaNota = { produtoId: 0, quantidade: 0 };
  
  mensagemErro = '';
  mensagemSucesso = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.carregarProdutos();
    this.carregarNotas();
  }

  carregarProdutos() {
    this.http.get<any[]>('http://localhost:5001/produtos').subscribe(data => this.produtos = data);
  }

  carregarNotas() {
    this.http.get<any[]>('http://localhost:5002/notas').subscribe(data => this.notas = data);
  }

  cadastrarProduto() {
    this.http.post('http://localhost:5001/produtos', this.novoProduto).subscribe(() => {
      this.mensagemSucesso = 'Produto cadastrado com sucesso!';
      this.carregarProdutos();
      this.novoProduto = { descricao: '', saldo: 0 };
    });
  }

  emitirNota() {
    this.mensagemErro = '';
    this.mensagemSucesso = '';
    
    this.http.post('http://localhost:5002/notas/emitir', this.novaNota)
      .pipe(
        catchError(erro => {
          this.mensagemErro = erro.error?.detail || erro.error?.erro || "Erro inesperado de comunicação.";
          return of(null);
        })
      )
      .subscribe(res => {
        if (res) {
          this.mensagemSucesso = 'Nota fiscal impressa com sucesso e estoque baixado!';
          this.carregarNotas();
          this.carregarProdutos();
        }
      });
  }
}