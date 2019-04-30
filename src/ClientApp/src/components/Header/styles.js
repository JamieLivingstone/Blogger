import styled from 'styled-components';

export const Header = styled.header`
  display: grid;
  grid-template-columns: 1fr 3fr;
  align-items: center;
  padding: 1rem 0;
`;

export const Nav = styled.nav`
  text-align: right;

  a {
    margin-right: 0.5rem;
    padding: 0.5rem 0;
    text-decoration: none;
    color: rgba(0, 0, 0, 0.3);

    &.active {
      color: #000;
    }

    &:last-of-type {
      margin-right: 0;
    }
  }
`;
